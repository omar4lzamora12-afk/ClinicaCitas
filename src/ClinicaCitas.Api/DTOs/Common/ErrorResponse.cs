namespace ClinicaCitas.Api.DTOs.Common;

/// <summary>
/// Estructura única de TODAS las respuestas de error (400, 404, 409, 415, 422, 500...).
/// Comparte con <see cref="ApiResponse{T}"/> los campos <c>exito</c>, <c>mensaje</c> y <c>marcaDeTiempo</c>.
/// </summary>
public sealed class ErrorResponse
{
    /// <summary>Siempre <c>false</c> en errores.</summary>
    public bool Exito => false;

    /// <summary>Código de estado HTTP (se repite en el cuerpo para facilitar el consumo).</summary>
    public int CodigoEstado { get; init; }

    /// <summary>Tipo de error en pocas palabras.</summary>
    public string Titulo { get; init; } = string.Empty;

    /// <summary>Explicación clara de qué salió mal.</summary>
    public string Mensaje { get; init; } = string.Empty;

    /// <summary>Detalle por campo en errores de validación: <c>{ "dni": ["El DNI debe..."] }</c>.</summary>
    public IDictionary<string, string[]>? Errores { get; init; }

    /// <summary>Ruta que originó el error.</summary>
    public string Ruta { get; init; } = string.Empty;

    /// <summary>Identificador de la petición, útil para buscar el error en los logs.</summary>
    public string IdTraza { get; init; } = string.Empty;

    /// <summary>Momento en que ocurrió el error.</summary>
    public DateTimeOffset MarcaDeTiempo { get; init; } = DateTimeOffset.Now;
}
