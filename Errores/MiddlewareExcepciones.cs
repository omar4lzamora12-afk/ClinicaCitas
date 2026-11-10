using System.Net;
using System.Text.Json;

namespace ClinicaApi.Errores
{
    public class MiddlewareExcepciones
    {
        private readonly RequestDelegate _siguiente; // Delegado que representa el siguiente componente del pipeline
        private readonly ILogger<MiddlewareExcepciones> _logger; // Para registrar el error (logging)

        public MiddlewareExcepciones(RequestDelegate siguiente, ILogger<MiddlewareExcepciones> logger)
        {
            _siguiente = siguiente;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext contexto) // Ejecuta tarea asincrona para procesar peticiones HTTP
        {
            try
            {
                // Deja que la petición pase hacia los controladores
                await _siguiente(contexto);
            }
            catch (Exception ex)
            {
                // ¡AQUÍ CAEN TODOS LOS ERRORES DE LA API!
                await ManejarErroresAsync(contexto, ex);
            }
        }

        private Task ManejarErroresAsync(HttpContext contexto, Exception excepcion)
        {
            int codigo;
            string mensaje;

            // Cada tipo de excepcion se traduce a su codigo HTTP correspondiente
            switch (excepcion)
            {
                case NotFoundException:
                    codigo = (int)HttpStatusCode.NotFound;            // 404
                    mensaje = excepcion.Message;
                    break;
                case ConflictException:
                    codigo = (int)HttpStatusCode.Conflict;            // 409
                    mensaje = excepcion.Message;
                    break;
                case ArgumentException:
                    codigo = (int)HttpStatusCode.BadRequest;          // 400
                    mensaje = excepcion.Message;
                    break;
                default:
                    codigo = (int)HttpStatusCode.InternalServerError; // 500
                    mensaje = "Ocurrió un error interno en el servidor."; // No se expone el detalle (seguridad)
                    break;
            }

            _logger.LogError(excepcion, "Error en la ruta {Ruta}", contexto.Request.Path.ToString());

            // La respuesta siempre será JSON
            contexto.Response.ContentType = "application/json";
            contexto.Response.StatusCode = codigo;

            // Armamos el objeto con el formato estandarizado
            var respuesta = new RespuestaError
            {
                CodigoEstado = codigo,
                Mensaje = mensaje,
                Ruta = contexto.Request.Path.ToString()
            };

            var json = JsonSerializer.Serialize(respuesta); // convierte respuesta a formato Json
            return contexto.Response.WriteAsync(json);      // se retorna Json al Postman
        }
    }
}
