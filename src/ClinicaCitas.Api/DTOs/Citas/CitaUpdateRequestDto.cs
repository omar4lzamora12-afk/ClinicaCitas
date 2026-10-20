namespace ClinicaCitas.Api.DTOs.Citas;

/// <summary>
/// Cuerpo del <c>PUT /api/v1/citas/{id}</c> (reprogramación). El paciente no se puede cambiar:
/// si fuera otra persona, corresponde registrar una cita nueva. Si <c>duracionMinutos</c> se omite, se conserva la actual.
/// </summary>
public sealed class CitaUpdateRequestDto : CitaBaseRequestDto
{
}
