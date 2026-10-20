using System.Text.Json;
using ClinicaCitas.Api.DTOs.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ClinicaCitas.Api.Middleware;

/// <summary>
/// Completa el manejo centralizado para los errores que NO son excepciones:
/// validaciones de DTOs (400) y respuestas vacías del framework (404 de ruta, 405, 415).
/// Así absolutamente todos los errores salen con la estructura de <see cref="ErrorResponse"/>.
/// </summary>
public static class RespuestasDeError
{
    /// <summary>Se ejecuta cuando <c>[ApiController]</c> detecta datos inválidos (DataAnnotations o conversión de tipos).</summary>
    public static IActionResult CrearRespuestaDeValidacion(ActionContext contexto)
    {
        var errores = new Dictionary<string, string[]>();

        foreach (var (clave, entrada) in contexto.ModelState)
        {
            if (entrada is null || entrada.Errors.Count == 0)
            {
                continue;
            }

            var campo = NormalizarCampo(clave);
            var esErrorDeJson = clave.StartsWith('$'); // el formateador JSON usa rutas como "$.pacienteId"
            var mensajes = entrada.Errors.Select(e => TraducirError(e, campo, esErrorDeJson));
            errores[campo] = errores.TryGetValue(campo, out var previos)
                ? previos.Concat(mensajes).Distinct().ToArray()
                : mensajes.Distinct().ToArray();
        }

        var respuesta = new ErrorResponse
        {
            CodigoEstado = StatusCodes.Status400BadRequest,
            Titulo = "Error de validación",
            Mensaje = "Uno o más campos no cumplen las reglas de validación. Revise el detalle en 'errores'.",
            Errores = errores,
            Ruta = contexto.HttpContext.Request.Path.ToString(),
            IdTraza = contexto.HttpContext.TraceIdentifier
        };

        return new BadRequestObjectResult(respuesta);
    }

    /// <summary>Se ejecuta con <c>UseStatusCodePages</c> cuando la respuesta tiene error pero no tiene cuerpo.</summary>
    public static Task EscribirRespuestaDeEstado(StatusCodeContext contexto)
    {
        var http = contexto.HttpContext;
        var codigo = http.Response.StatusCode;

        var (titulo, mensaje) = codigo switch
        {
            StatusCodes.Status400BadRequest => ("Solicitud incorrecta", "La solicitud no pudo ser interpretada. Revise los datos enviados."),
            StatusCodes.Status404NotFound => ("Ruta no encontrada", "La ruta solicitada no existe en esta API. Consulte /swagger para ver los endpoints disponibles."),
            StatusCodes.Status405MethodNotAllowed => ("Método no permitido", "El método HTTP utilizado no está permitido para esta ruta."),
            StatusCodes.Status415UnsupportedMediaType => ("Tipo de contenido no soportado", "Envíe el cuerpo en formato JSON con el encabezado Content-Type: application/json."),
            _ => ("Error en la solicitud", "No fue posible procesar la solicitud.")
        };

        return http.Response.WriteAsJsonAsync(new ErrorResponse
        {
            CodigoEstado = codigo,
            Titulo = titulo,
            Mensaje = mensaje,
            Ruta = http.Request.Path.ToString(),
            IdTraza = http.TraceIdentifier
        });
    }

    /// <summary>Convierte claves como "$.fechaHora" o "FechaHora" en "fechaHora" (igual que en el JSON).</summary>
    private static string NormalizarCampo(string clave)
    {
        var limpia = clave.TrimStart('$', '.');
        return string.IsNullOrWhiteSpace(limpia)
            ? "cuerpo"
            : string.Join('.', limpia.Split('.').Select(parte => JsonNamingPolicy.CamelCase.ConvertName(parte)));
    }

    /// <summary>Reemplaza los mensajes técnicos de la deserialización JSON por uno comprensible.</summary>
    private static string TraducirError(ModelError error, string campo, bool esErrorDeJson)
    {
        if (!esErrorDeJson && error.Exception is null)
        {
            return error.ErrorMessage; // mensaje de DataAnnotations (ya está en español)
        }

        return campo == "cuerpo"
            ? "El cuerpo de la solicitud no contiene un JSON válido."
            : $"El valor enviado para '{campo}' no tiene el formato esperado (revise el tipo de dato).";
    }
}
