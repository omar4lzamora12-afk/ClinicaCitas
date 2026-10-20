namespace ClinicaCitas.Api.DTOs.Pacientes;

/// <summary>Representación de un paciente que devuelve la API (nunca se expone la entidad directamente).</summary>
public sealed class PacienteResponseDto
{
    public int Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string Apellidos { get; init; } = string.Empty;
    public string NombreCompleto { get; init; } = string.Empty;
    public string Dni { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateOnly FechaNacimiento { get; init; }
    public string? Direccion { get; init; }
    public DateTime FechaCreacion { get; init; }
    public DateTime? FechaActualizacion { get; init; }
}
