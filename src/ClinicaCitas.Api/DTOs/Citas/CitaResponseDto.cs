using ClinicaCitas.Api.Models.Enums;

namespace ClinicaCitas.Api.DTOs.Citas;

/// <summary>
/// Cita lista para mostrar: además de los Id incluye los nombres del paciente y del médico
/// (equivalente a un JOIN), para que el cliente no tenga que hacer más consultas.
/// </summary>
public sealed class CitaResponseDto
{
    public int Id { get; init; }
    public int PacienteId { get; init; }
    public string Paciente { get; init; } = string.Empty;
    public int MedicoId { get; init; }
    public string Medico { get; init; } = string.Empty;
    public string Especialidad { get; init; } = string.Empty;
    public DateTime FechaHora { get; init; }
    public DateTime FechaFin { get; init; }
    public int DuracionMinutos { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public string? Observaciones { get; init; }
    public EstadoCita Estado { get; init; }
    public string? MotivoCancelacion { get; init; }
    public DateTime FechaCreacion { get; init; }
    public DateTime? FechaActualizacion { get; init; }
}
