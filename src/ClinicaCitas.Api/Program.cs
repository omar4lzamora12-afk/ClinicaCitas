using ClinicaCitas.Api.Configuration;
using ClinicaCitas.Api.Extensions;
using ClinicaCitas.Api.Middleware;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// 1) Configuración tipada: sección "Clinica" de appsettings.json → ConfiguracionClinica
builder.Services.Configure<ConfiguracionClinica>(builder.Configuration.GetSection(ConfiguracionClinica.Seccion));

// 2) Controladores, formato JSON y validación automática ([ApiController] + DataAnnotations)
builder.Services
    .AddControllers(opciones =>
    {
        MensajesEnEspanol.Aplicar(opciones.ModelBindingMessageProvider);
        opciones.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddJsonOptions(opciones => ConfiguracionJson.Aplicar(opciones.JsonSerializerOptions))
    .ConfigureApiBehaviorOptions(opciones =>
    {
        opciones.InvalidModelStateResponseFactory = RespuestasDeError.CrearRespuestaDeValidacion;
        opciones.SuppressMapClientErrors = true; // los 4xx sin cuerpo los completa UseStatusCodePages con nuestro formato
    });

builder.Services.ConfigureHttpJsonOptions(opciones => ConfiguracionJson.Aplicar(opciones.SerializerOptions));

// 3) Capas de la aplicación: repositorios (datos en memoria) y servicios (lógica de negocio)
builder.Services.AddCapasDeLaAplicacion();

// 4) Manejo centralizado de excepciones
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 5) Documentación Swagger
builder.Services.AddDocumentacionSwagger();

var app = builder.Build();

// ── Pipeline HTTP (el orden importa) ──
app.UseExceptionHandler();                                      // captura cualquier excepción no controlada
app.UseStatusCodePages(RespuestasDeError.EscribirRespuestaDeEstado); // 404/405/415 con el mismo formato JSON
app.UseDocumentacionSwagger();                                  // /swagger

app.MapGet("/", () => Results.Ok(new
{
    nombre = "API de Gestión de Citas Médicas",
    version = "v1",
    estado = "En línea",
    documentacion = "/swagger",
    recursos = new[] { "/api/v1/pacientes", "/api/v1/medicos", "/api/v1/citas" }
})).ExcludeFromDescription();

app.MapControllers();

app.SembrarDatosSiCorresponde();

app.Run();
