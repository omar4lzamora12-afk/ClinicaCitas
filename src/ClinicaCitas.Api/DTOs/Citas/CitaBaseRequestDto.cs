using System.ComponentModel.DataAnnotations;
using ClinicaCitas.Api.Validation;

namespace ClinicaCitas.Api.DTOs.Citas;

/// <summary>
/// Campos y validaciones comunes a registrar y a reprogramar una cita.
/// Las clases hijas heredan las anotaciones (REUTILIZACIÓN por herencia).
/// </summary>
public abstract class CitaBaseRequestDto
{
    /// <summary>Id del médico que atenderá. Debe existir y estar activo.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un Id de médico válido (mayor que 0).")]
    public int MedicoId { get; init; }

    /// <summary>Inicio de la cita, en hora local y sin zona horaria. Ejemplo: "2026-11-10T09:30:00".</summary>
    [Required(ErrorMessage = "La fecha y hora de la cita son obligatorias.")]
    [FechaFutura(ErrorMessage = "La fecha y hora de la cita deben ser posteriores al momento actual.")]
    public DateTime? FechaHora { get; init; }

    /// <summary>Duración en minutos (15 a 120). Si se omite se usa la duración por defecto de la clínica.</summary>
    [Range(15, 120, ErrorMessage = "La duración debe estar entre 15 y 120 minutos.")]
    public int? DuracionMinutos { get; init; }

    /// <summary>Motivo de la consulta.</summary>
    [Required(ErrorMessage = "El motivo de la consulta es obligatorio.")]
    [StringLength(250, MinimumLength = 5, ErrorMessage = "El motivo debe tener entre 5 y 250 caracteres.")]
    public string Motivo { get; init; } = string.Empty;

    /// <summary>Notas adicionales (opcional).</summary>
    [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
    public string? Observaciones { get; init; }
}
