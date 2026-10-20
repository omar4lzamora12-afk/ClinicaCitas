using ClinicaCitas.Api.Models.Enums;

namespace ClinicaCitas.Api.DTOs.Citas;

/// <summary>Filtros opcionales (query string) del <c>GET /api/v1/citas</c>. Se pueden combinar.</summary>
public sealed class CitaFiltroDto
{
    /// <summary>Solo citas de ese día (AAAA-MM-DD).</summary>
    public DateOnly? Fecha { get; init; }

    /// <summary>Solo citas en ese estado: Programada, Confirmada, Atendida o Cancelada.</summary>
    public EstadoCita? Estado { get; init; }

    /// <summary>Solo citas de ese médico.</summary>
    public int? MedicoId { get; init; }

    /// <summary>Solo citas de ese paciente.</summary>
    public int? PacienteId { get; init; }
}
