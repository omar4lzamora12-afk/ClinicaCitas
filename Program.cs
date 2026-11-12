using ClinicaApi.Errores;
using ClinicaApi.Servicios;

// 1. Inicializa el constructor de la aplicación web, configurando los ajustes por defecto
var Builder = WebApplication.CreateBuilder(args);

// 2. Registra los controladores de la API
Builder.Services.AddControllers();

// 3. Registra los servicios.
// Se usa AddSingleton porque los datos viven en listas en memoria y deben mantenerse
// mientras la aplicación esté en ejecución.
Builder.Services.AddSingleton<IPacienteService, PacienteService>();
Builder.Services.AddSingleton<IMedicoService, MedicoService>();
Builder.Services.AddSingleton<ICitaService, CitaService>();

// 4. Construye la instancia de la aplicación (App)
var App = Builder.Build();

// 5. Middleware de excepciones: debe ir ANTES de MapControllers para atrapar cualquier error
App.UseMiddleware<MiddlewareExcepciones>();

// 6. Mapea la ruta de los controladores mediante los atributos [Route] y [HttpGet/HttpPost]
App.MapControllers();

// 7. Inicia el servidor y escucha las peticiones
App.Run();
