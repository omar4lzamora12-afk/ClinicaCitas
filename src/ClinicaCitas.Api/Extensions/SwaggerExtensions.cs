using System.Reflection;
using Microsoft.OpenApi.Models;

namespace ClinicaCitas.Api.Extensions;

/// <summary>Configuración de Swagger / OpenAPI: documentación interactiva generada desde el código.</summary>
public static class SwaggerExtensions
{
    public static IServiceCollection AddDocumentacionSwagger(this IServiceCollection servicios)
    {
        servicios.AddEndpointsApiExplorer();
        servicios.AddSwaggerGen(opciones =>
        {
            opciones.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API de Gestión de Citas Médicas",
                Version = "v1",
                Description = "API RESTful para registrar, consultar, reprogramar, cancelar y eliminar citas médicas de una clínica privada. " +
                              "Los datos se guardan en memoria (sin base de datos) y se reinician al apagar la aplicación."
            });

            // Incluye los comentarios /// de controladores y DTOs en la documentación.
            var archivoXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var rutaXml = Path.Combine(AppContext.BaseDirectory, archivoXml);
            if (File.Exists(rutaXml))
            {
                opciones.IncludeXmlComments(rutaXml, includeControllerXmlComments: true);
            }
        });

        return servicios;
    }

    public static WebApplication UseDocumentacionSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(opciones =>
        {
            opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Citas Médicas v1");
            opciones.DocumentTitle = "Clínica · API de Citas Médicas";
        });

        return app;
    }
}
