namespace ClinicaCitas.Api.Exceptions;

/// <summary>
/// Clase base de las excepciones "esperadas" de la aplicación. Cada una lleva el código HTTP que
/// le corresponde, de modo que el manejador global (<c>GlobalExceptionHandler</c>) pueda convertirla
/// en una respuesta clara sin necesidad de try/catch en controladores ni servicios.
/// </summary>
public abstract class AppException : Exception
{
    /// <summary>Código de estado HTTP asociado (404, 409, 422, ...).</summary>
    public int CodigoEstado { get; }

    /// <summary>Título corto del tipo de error.</summary>
    public string Titulo { get; }

    protected AppException(int codigoEstado, string titulo, string mensaje) : base(mensaje)
    {
        CodigoEstado = codigoEstado;
        Titulo = titulo;
    }
}
