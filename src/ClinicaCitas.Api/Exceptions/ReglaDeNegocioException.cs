namespace ClinicaCitas.Api.Exceptions;

/// <summary>
/// Los datos están bien formados pero incumplen una regla del negocio (fuera del horario de atención,
/// médico inactivo, etc.). Se traduce a HTTP 422.
/// </summary>
public sealed class ReglaDeNegocioException : AppException
{
    public ReglaDeNegocioException(string mensaje)
        : base(StatusCodes.Status422UnprocessableEntity, "Regla de negocio incumplida", mensaje)
    {
    }
}
