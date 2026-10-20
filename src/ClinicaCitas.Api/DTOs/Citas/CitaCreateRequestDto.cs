using System.ComponentModel.DataAnnotations;

namespace ClinicaCitas.Api.DTOs.Citas;

/// <summary>Cuerpo del <c>POST /api/v1/citas</c>: agrega el paciente a los campos comunes.</summary>
public sealed class CitaCreateRequestDto : CitaBaseRequestDto
{
    /// <summary>Id del paciente que solicita la cita. Debe existir.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un Id de paciente válido (mayor que 0).")]
    public int PacienteId { get; init; }
}
