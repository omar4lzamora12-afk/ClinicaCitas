using System.ComponentModel.DataAnnotations;

namespace ClinicaCitas.Api.DTOs.Citas;

/// <summary>Cuerpo del <c>PATCH /api/v1/citas/{id}/cancelar</c>.</summary>
public sealed class CitaCancelRequestDto
{
    /// <summary>Razón por la que se cancela la cita.</summary>
    [Required(ErrorMessage = "El motivo de la cancelación es obligatorio.")]
    [StringLength(250, MinimumLength = 5, ErrorMessage = "El motivo de la cancelación debe tener entre 5 y 250 caracteres.")]
    public string Motivo { get; init; } = string.Empty;
}
