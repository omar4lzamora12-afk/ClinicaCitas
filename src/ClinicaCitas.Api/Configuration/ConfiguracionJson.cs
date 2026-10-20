using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClinicaCitas.Api.Configuration;

/// <summary>
/// Reglas de serialización JSON compartidas por los controladores y por el manejador de errores,
/// para que TODAS las respuestas de la API tengan exactamente el mismo formato.
/// </summary>
public static class ConfiguracionJson
{
    public static void Aplicar(JsonSerializerOptions opciones)
    {
        opciones.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;                  // "fechaHora"
        opciones.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;      // omite campos nulos
        opciones.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;             // tildes y ñ legibles

        if (!opciones.Converters.OfType<JsonStringEnumConverter>().Any())
        {
            opciones.Converters.Add(new JsonStringEnumConverter());                   // "Programada" en vez de 1
        }
    }
}
