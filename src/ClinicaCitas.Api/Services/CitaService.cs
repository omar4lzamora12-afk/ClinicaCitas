using ClinicaCitas.Api.Configuration;
using ClinicaCitas.Api.DTOs.Citas;
using ClinicaCitas.Api.Exceptions;
using ClinicaCitas.Api.Mappings;
using ClinicaCitas.Api.Models.Entities;
using ClinicaCitas.Api.Models.Enums;
using ClinicaCitas.Api.Repositories;
using Microsoft.Extensions.Options;

namespace ClinicaCitas.Api.Services;

/// <summary>
/// Núcleo del negocio. Orquesta repositorios y entidades para aplicar las reglas de la clínica:
/// <list type="number">
///   <item><description>El paciente y el médico deben existir (y el médico estar activo).</description></item>
///   <item><description>La cita debe caer dentro del horario de atención y no en domingo.</description></item>
///   <item><description>El médico y el paciente no pueden tener dos citas vigentes que se crucen.</description></item>
///   <item><description>Los cambios de estado siguen el ciclo Programada → Confirmada → Atendida / Cancelada.</description></item>
/// </list>
/// No usa códigos HTTP: lanza excepciones de negocio que el manejador global traduce.
/// </summary>
public sealed class CitaService : ICitaService
{
    private readonly ICitaRepository _citas;
    private readonly IPacienteRepository _pacientes;
    private readonly IMedicoRepository _medicos;
    private readonly ConfiguracionClinica _configuracion;
    private readonly ILogger<CitaService> _logger;

    public CitaService(ICitaRepository citas, IPacienteRepository pacientes, IMedicoRepository medicos,
                       IOptions<ConfiguracionClinica> configuracion, ILogger<CitaService> logger)
    {
        _citas = citas;
        _pacientes = pacientes;
        _medicos = medicos;
        _configuracion = configuracion.Value;
        _logger = logger;
    }

    // ───────────────────────────── Consultas ─────────────────────────────

    public IReadOnlyList<CitaResponseDto> Listar(CitaFiltroDto filtro)
    {
        var citas = _citas.Buscar(c =>
            (filtro.Estado is null || c.Estado == filtro.Estado) &&
            (filtro.MedicoId is null || c.MedicoId == filtro.MedicoId) &&
            (filtro.PacienteId is null || c.PacienteId == filtro.PacienteId) &&
            (filtro.Fecha is null || DateOnly.FromDateTime(c.FechaHora) == filtro.Fecha));

        return MapearLista(citas);
    }

    public CitaResponseDto ObtenerPorId(int id) => MapearCita(ObtenerCita(id));

    public IReadOnlyList<CitaResponseDto> ListarPorPaciente(int pacienteId, EstadoCita? estado)
    {
        ObtenerPaciente(pacienteId); // lanza 404 si el paciente no existe
        return MapearLista(_citas.ObtenerPorPaciente(pacienteId).Where(c => estado is null || c.Estado == estado));
    }

    public IReadOnlyList<CitaResponseDto> ListarPorMedico(int medicoId, DateOnly? fecha)
    {
        ObtenerMedico(medicoId); // lanza 404 si el médico no existe
        return MapearLista(_citas.ObtenerPorMedico(medicoId)
            .Where(c => fecha is null || DateOnly.FromDateTime(c.FechaHora) == fecha));
    }

    // ───────────────────────────── Comandos ─────────────────────────────

    public CitaResponseDto Registrar(CitaCreateRequestDto solicitud) =>
        TransaccionSimulada.Ejecutar(() =>
        {
            var paciente = ObtenerPaciente(solicitud.PacienteId);
            var medico = ObtenerMedicoActivo(solicitud.MedicoId);
            var inicio = solicitud.FechaHora!.Value;
            var duracion = solicitud.DuracionMinutos ?? _configuracion.DuracionCitaPorDefectoMinutos;

            ValidarHorarioDeAtencion(inicio, duracion);
            ValidarDisponibilidad(medico.Id, paciente.Id, inicio, inicio.AddMinutes(duracion), excluirCitaId: null);

            var cita = new Cita(paciente.Id, medico.Id, inicio, duracion, solicitud.Motivo, solicitud.Observaciones);
            var guardada = _citas.Agregar(cita);

            _logger.LogInformation(
                "Cita {CitaId} registrada: paciente {PacienteId}, médico {MedicoId}, {FechaHora:yyyy-MM-dd HH:mm}.",
                guardada.Id, paciente.Id, medico.Id, guardada.FechaHora);
            return guardada.ToResponseDto(paciente, medico);
        });

    public CitaResponseDto Actualizar(int id, CitaUpdateRequestDto solicitud) =>
        TransaccionSimulada.Ejecutar(() =>
        {
            var cita = ObtenerCita(id);
            var paciente = ObtenerPaciente(cita.PacienteId);
            var medico = ObtenerMedicoActivo(solicitud.MedicoId);
            var duracion = solicitud.DuracionMinutos ?? cita.DuracionMinutos;

            // La propia entidad rechaza (409) la modificación si la cita ya no está vigente.
            // Trabajamos sobre una copia: si alguna validación falla, nada se guarda.
            cita.Reprogramar(medico.Id, solicitud.FechaHora!.Value, duracion, solicitud.Motivo, solicitud.Observaciones);

            ValidarHorarioDeAtencion(cita.FechaHora, cita.DuracionMinutos);
            ValidarDisponibilidad(medico.Id, paciente.Id, cita.FechaHora, cita.FechaFin, excluirCitaId: cita.Id);

            return Guardar(cita, paciente, medico, "reprogramada");
        });

    public CitaResponseDto Confirmar(int id) => CambiarEstado(id, cita => cita.Confirmar(), "confirmada");

    public CitaResponseDto Atender(int id) => CambiarEstado(id, cita => cita.Atender(), "marcada como atendida");

    public CitaResponseDto Cancelar(int id, CitaCancelRequestDto solicitud) =>
        CambiarEstado(id, cita => cita.Cancelar(solicitud.Motivo), "cancelada");

    public void Eliminar(int id) =>
        TransaccionSimulada.Ejecutar(() =>
        {
            var cita = ObtenerCita(id);

            if (cita.Estado == EstadoCita.Atendida)
            {
                throw new ConflictoException(
                    "No se puede eliminar una cita atendida porque forma parte del historial clínico del paciente.");
            }

            _citas.Eliminar(id);
            _logger.LogInformation("Cita {CitaId} eliminada.", id);
        });

    // ───────────────────────────── Reglas de negocio ─────────────────────────────

    /// <summary>Aplica una transición de estado; la entidad valida que sea legal.</summary>
    private CitaResponseDto CambiarEstado(int id, Action<Cita> transicion, string accion) =>
        TransaccionSimulada.Ejecutar(() =>
        {
            var cita = ObtenerCita(id);
            transicion(cita);
            return Guardar(cita, ObtenerPaciente(cita.PacienteId), ObtenerMedico(cita.MedicoId), accion);
        });

    private void ValidarHorarioDeAtencion(DateTime inicio, int duracionMinutos)
    {
        if (inicio.DayOfWeek == DayOfWeek.Sunday && !_configuracion.AtiendeDomingos)
        {
            throw new ReglaDeNegocioException("La clínica no atiende los domingos. Elija otro día.");
        }

        var fin = inicio.AddMinutes(duracionMinutos);
        var terminaElMismoDia = fin.Date == inicio.Date;

        if (inicio.TimeOfDay < _configuracion.HoraApertura || !terminaElMismoDia || fin.TimeOfDay > _configuracion.HoraCierre)
        {
            throw new ReglaDeNegocioException(
                $"La cita debe realizarse dentro del horario de atención ({Formato(_configuracion.HoraApertura)} a {Formato(_configuracion.HoraCierre)}) y terminar antes del cierre.");
        }
    }

    private void ValidarDisponibilidad(int medicoId, int pacienteId, DateTime inicio, DateTime fin, int? excluirCitaId)
    {
        if (_citas.HayConflictoDeMedico(medicoId, inicio, fin, excluirCitaId))
        {
            throw new ConflictoException(
                "El médico ya tiene una cita vigente que se cruza con ese horario. Elija otra hora u otro médico.");
        }

        if (_citas.HayConflictoDePaciente(pacienteId, inicio, fin, excluirCitaId))
        {
            throw new ConflictoException("El paciente ya tiene otra cita vigente que se cruza con ese horario.");
        }
    }

    // ───────────────────────────── Utilidades ─────────────────────────────

    private CitaResponseDto Guardar(Cita cita, Paciente paciente, Medico medico, string accion)
    {
        var guardada = _citas.Actualizar(cita) ?? throw new RecursoNoEncontradoException("la cita", cita.Id);
        _logger.LogInformation("Cita {CitaId} {Accion}. Estado actual: {Estado}.", guardada.Id, accion, guardada.Estado);
        return guardada.ToResponseDto(paciente, medico);
    }

    private Cita ObtenerCita(int id) =>
        _citas.ObtenerPorId(id) ?? throw new RecursoNoEncontradoException("la cita", id);

    private Paciente ObtenerPaciente(int id) =>
        _pacientes.ObtenerPorId(id) ?? throw new RecursoNoEncontradoException("el paciente", id);

    private Medico ObtenerMedico(int id) =>
        _medicos.ObtenerPorId(id) ?? throw new RecursoNoEncontradoException("el médico", id);

    private Medico ObtenerMedicoActivo(int id)
    {
        var medico = ObtenerMedico(id);
        if (!medico.Activo)
        {
            throw new ReglaDeNegocioException($"El médico {medico.NombreCompleto} no está activo y no puede recibir citas.");
        }
        return medico;
    }

    private IReadOnlyList<CitaResponseDto> MapearLista(IEnumerable<Cita> citas) =>
        citas.OrderBy(c => c.FechaHora).ThenBy(c => c.Id).Select(MapearCita).ToList();

    private CitaResponseDto MapearCita(Cita cita) =>
        cita.ToResponseDto(ObtenerPaciente(cita.PacienteId), ObtenerMedico(cita.MedicoId));

    private static string Formato(TimeSpan hora) => hora.ToString(@"hh\:mm");
}
