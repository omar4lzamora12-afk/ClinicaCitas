using ClinicaCitas.Api.DTOs.Common;
using ClinicaCitas.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace ClinicaCitas.Api.Middleware;

/// <summary>
/// MANEJO CENTRALIZADO DE EXCEPCIONES. Toda excepción que escape de un controlador o de un servicio
/// termina aquí (se registra con <c>AddExceptionHandler</c> y se activa con <c>UseExceptionHandler</c>).
/// <list type="bullet">
///   <item><description>Excepciones de negocio (<see cref="AppException"/>): 404, 409, 422 con su mensaje claro.</description></item>
///   <item><description>Cualquier otra: 500 con un mensaje genérico (el detalle técnico solo va al log, nunca al cliente).</description></item>
/// </list>
/// Gracias a esto los controladores y servicios no necesitan bloques try/catch.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ErrorResponse respuesta;

        if (exception is AppException esperada)
        {
            _logger.LogWarning("{Titulo}: {Mensaje} [{Metodo} {Ruta}]",
                esperada.Titulo, esperada.Message, httpContext.Request.Method, httpContext.Request.Path);

            respuesta = Construir(httpContext, esperada.CodigoEstado, esperada.Titulo, esperada.Message);
        }
        else
        {
            _logger.LogError(exception, "Error no controlado en {Metodo} {Ruta}",
                httpContext.Request.Method, httpContext.Request.Path);

            respuesta = Construir(httpContext, StatusCodes.Status500InternalServerError, "Error interno del servidor",
                "Ocurrió un error inesperado. Intente nuevamente; si el problema continúa, informe el idTraza al administrador.");
        }

        httpContext.Response.StatusCode = respuesta.CodigoEstado;
        await httpContext.Response.WriteAsJsonAsync(respuesta, cancellationToken);
        return true; // true = "ya la manejé, no sigas buscando otro manejador"
    }

    private static ErrorResponse Construir(HttpContext contexto, int codigo, string titulo, string mensaje) => new()
    {
        CodigoEstado = codigo,
        Titulo = titulo,
        Mensaje = mensaje,
        Ruta = contexto.Request.Path.ToString(),
        IdTraza = contexto.TraceIdentifier
    };
}
