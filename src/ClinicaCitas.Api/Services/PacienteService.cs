using ClinicaCitas.Api.DTOs.Pacientes;
using ClinicaCitas.Api.Exceptions;
using ClinicaCitas.Api.Mappings;
using ClinicaCitas.Api.Models.Entities;
using ClinicaCitas.Api.Repositories;

namespace ClinicaCitas.Api.Services;

/// <summary>
/// Lógica de negocio de pacientes: unicidad del DNI e integridad referencial con las citas.
/// No conoce HTTP (no usa códigos de estado): solo lanza excepciones de negocio.
/// </summary>
public sealed class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _pacientes;
    private readonly ICitaRepository _citas;
    private readonly ILogger<PacienteService> _logger;

    public PacienteService(IPacienteRepository pacientes, ICitaRepository citas, ILogger<PacienteService> logger)
    {
        _pacientes = pacientes;
        _citas = citas;
        _logger = logger;
    }

    public IReadOnlyList<PacienteResponseDto> ObtenerTodos() =>
        _pacientes.ObtenerTodos().Select(p => p.ToResponseDto()).ToList();

    public PacienteResponseDto ObtenerPorId(int id) => ObtenerEntidad(id).ToResponseDto();

    public PacienteResponseDto Registrar(PacienteRequestDto solicitud) =>
        TransaccionSimulada.Ejecutar(() =>
        {
            VerificarDniDisponible(solicitud.Dni, excluirPacienteId: null);

            var paciente = new Paciente(solicitud.Nombres, solicitud.Apellidos, solicitud.Dni, solicitud.Telefono,
                solicitud.Email, solicitud.FechaNacimiento!.Value, solicitud.Direccion);

            var guardado = _pacientes.Agregar(paciente);
            _logger.LogInformation("Paciente {PacienteId} registrado.", guardado.Id);
            return guardado.ToResponseDto();
        });

    public PacienteResponseDto Actualizar(int id, PacienteRequestDto solicitud) =>
        TransaccionSimulada.Ejecutar(() =>
        {
            var paciente = ObtenerEntidad(id);
            VerificarDniDisponible(solicitud.Dni, excluirPacienteId: id);

            paciente.Actualizar(solicitud.Nombres, solicitud.Apellidos, solicitud.Dni, solicitud.Telefono,
                solicitud.Email, solicitud.FechaNacimiento!.Value, solicitud.Direccion);

            var guardado = _pacientes.Actualizar(paciente) ?? throw new RecursoNoEncontradoException("el paciente", id);
            _logger.LogInformation("Paciente {PacienteId} actualizado.", id);
            return guardado.ToResponseDto();
        });

    public void Eliminar(int id) =>
        TransaccionSimulada.Ejecutar(() =>
        {
            var paciente = ObtenerEntidad(id);

            // Integridad referencial: igual que una clave foránea sin eliminación en cascada.
            var cantidadDeCitas = _citas.Contar(c => c.PacienteId == id);
            if (cantidadDeCitas > 0)
            {
                throw new ConflictoException(
                    $"No se puede eliminar al paciente {paciente.NombreCompleto}: tiene {cantidadDeCitas} cita(s) registrada(s). Elimine primero sus citas.");
            }

            _pacientes.Eliminar(id);
            _logger.LogInformation("Paciente {PacienteId} eliminado.", id);
        });

    private Paciente ObtenerEntidad(int id) =>
        _pacientes.ObtenerPorId(id) ?? throw new RecursoNoEncontradoException("el paciente", id);

    /// <summary>Simula una restricción UNIQUE sobre el DNI.</summary>
    private void VerificarDniDisponible(string dni, int? excluirPacienteId)
    {
        var existente = _pacientes.ObtenerPorDni(dni.Trim());
        if (existente is not null && existente.Id != excluirPacienteId)
        {
            throw new ConflictoException($"Ya existe un paciente registrado con el DNI {dni}.");
        }
    }
}
