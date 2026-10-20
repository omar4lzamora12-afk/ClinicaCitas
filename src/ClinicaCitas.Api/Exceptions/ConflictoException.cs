namespace ClinicaCitas.Api.Exceptions;

/// <summary>
/// La operación choca con el estado actual de los datos (duplicados, horario ocupado, transición de
/// estado no permitida, registros dependientes). Se traduce a HTTP 409.
/// </summary>
public sealed class ConflictoException : AppException
{
    public ConflictoException(string mensaje)
        : base(StatusCodes.Status409Conflict, "Conflicto con el estado actual del recurso", mensaje)
    {
    }
}
