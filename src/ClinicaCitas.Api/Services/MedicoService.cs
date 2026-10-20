using ClinicaCitas.Api.DTOs.Medicos;
using ClinicaCitas.Api.Exceptions;
using ClinicaCitas.Api.Mappings;
using ClinicaCitas.Api.Models.Entities;
using ClinicaCitas.Api.Repositories;

namespace ClinicaCitas.Api.Services;

/// <summary>Lógica de negocio de médicos: unicidad de DNI y colegiatura, e integridad con las citas.</summary>
public sealed class MedicoService : IMedicoService
{
    private readonly IMedicoRepository _medicos;
    private readonly ICitaRepository _citas;
    private readonly ILogger<MedicoService> _logger;

    public MedicoService(IMedicoRepository medicos, ICitaRepository citas, ILogger<MedicoService> logger)
    {
        _medicos = medicos;
        _citas = citas;
        _logger = logger;
    }

    public IReadOnlyList<MedicoResponseDto> ObtenerTodos(string? especialidad = null)
    {
        var medicos = string.IsNullOrWhiteSpace(especialidad)
            ? _medicos.ObtenerTodos()
            : _medicos.Buscar(m => m.Especialidad.Contains(especialidad.Trim(), StringComparison.OrdinalIgnoreCase));

        return medicos.Select(m => m.ToResponseDto()).ToList();
    }

    public MedicoResponseDto ObtenerPorId(int id) => ObtenerEntidad(id).ToResponseDto();

    public MedicoResponseDto Registrar(MedicoRequestDto solicitud) =>
        TransaccionSimulada.Ejecutar(() =>
        {
            VerificarUnicidad(solicitud, excluirMedicoId: null);

            var medico = new Medico(solicitud.Nombres, solicitud.Apellidos, solicitud.Dni, solicitud.Telefono,
                solicitud.Email, solicitud.NumeroColegiatura, solicitud.Especialidad, solicitud.Activo);

            var guardado = _medicos.Agregar(medico);
            _logger.LogInformation("Médico {MedicoId} registrado.", guardado.Id);
            return guardado.ToResponseDto();
        });

    public MedicoResponseDto Actualizar(int id, MedicoRequestDto solicitud) =>
        TransaccionSimulada.Ejecutar(() =>
        {
            var medico = ObtenerEntidad(id);
            VerificarUnicidad(solicitud, excluirMedicoId: id);

            medico.Actualizar(solicitud.Nombres, solicitud.Apellidos, solicitud.Dni, solicitud.Telefono,
                solicitud.Email, solicitud.NumeroColegiatura, solicitud.Especialidad, solicitud.Activo);

            var guardado = _medicos.Actualizar(medico) ?? throw new RecursoNoEncontradoException("el médico", id);
            _logger.LogInformation("Médico {MedicoId} actualizado.", id);
            return guardado.ToResponseDto();
        });

    public void Eliminar(int id) =>
        TransaccionSimulada.Ejecutar(() =>
        {
            var medico = ObtenerEntidad(id);

            var cantidadDeCitas = _citas.Contar(c => c.MedicoId == id);
            if (cantidadDeCitas > 0)
            {
                throw new ConflictoException(
                    $"No se puede eliminar al médico {medico.NombreCompleto}: tiene {cantidadDeCitas} cita(s) registrada(s). Elimine primero sus citas o desactívelo (activo = false).");
            }

            _medicos.Eliminar(id);
            _logger.LogInformation("Médico {MedicoId} eliminado.", id);
        });

    private Medico ObtenerEntidad(int id) =>
        _medicos.ObtenerPorId(id) ?? throw new RecursoNoEncontradoException("el médico", id);

    /// <summary>Simula las restricciones UNIQUE de DNI y número de colegiatura.</summary>
    private void VerificarUnicidad(MedicoRequestDto solicitud, int? excluirMedicoId)
    {
        var porDni = _medicos.ObtenerPorDni(solicitud.Dni.Trim());
        if (porDni is not null && porDni.Id != excluirMedicoId)
        {
            throw new ConflictoException($"Ya existe un médico registrado con el DNI {solicitud.Dni}.");
        }

        var porColegiatura = _medicos.ObtenerPorColegiatura(solicitud.NumeroColegiatura.Trim());
        if (porColegiatura is not null && porColegiatura.Id != excluirMedicoId)
        {
            throw new ConflictoException($"Ya existe un médico registrado con la colegiatura {solicitud.NumeroColegiatura}.");
        }
    }
}
