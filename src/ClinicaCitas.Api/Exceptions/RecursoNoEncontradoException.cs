namespace ClinicaCitas.Api.Exceptions;

/// <summary>El recurso solicitado no existe. Se traduce a HTTP 404.</summary>
public sealed class RecursoNoEncontradoException : AppException
{
    public RecursoNoEncontradoException(string recurso, int id)
        : base(StatusCodes.Status404NotFound, "Recurso no encontrado", $"No se encontró {recurso} con Id {id}.")
    {
    }
}
