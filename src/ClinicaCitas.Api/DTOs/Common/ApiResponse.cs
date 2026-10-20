namespace ClinicaCitas.Api.DTOs.Common;

/// <summary>
/// Envoltorio estándar de TODAS las respuestas exitosas. Gracias a él, quien consume la API siempre
/// encuentra los mismos campos: <c>exito</c>, <c>mensaje</c>, <c>datos</c> y <c>marcaDeTiempo</c>.
/// </summary>
/// <typeparam name="T">Tipo del contenido devuelto en <see cref="Datos"/>.</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>Siempre <c>true</c> en respuestas correctas.</summary>
    public bool Exito { get; init; } = true;

    /// <summary>Mensaje legible que describe lo ocurrido.</summary>
    public string Mensaje { get; init; } = string.Empty;

    /// <summary>Contenido solicitado (un objeto o una lista).</summary>
    public T? Datos { get; init; }

    /// <summary>Momento en que el servidor generó la respuesta.</summary>
    public DateTimeOffset MarcaDeTiempo { get; init; } = DateTimeOffset.Now;

    /// <summary>Fábrica para construir una respuesta exitosa.</summary>
    public static ApiResponse<T> Correcta(T datos, string mensaje) => new() { Mensaje = mensaje, Datos = datos };
}
