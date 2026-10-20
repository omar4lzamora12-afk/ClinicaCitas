using ClinicaCitas.Api.Data;
using ClinicaCitas.Api.Repositories;
using ClinicaCitas.Api.Services;

namespace ClinicaCitas.Api.Extensions;

/// <summary>Registro de dependencias (inyección de dependencias) en un solo lugar, para mantener Program.cs limpio.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCapasDeLaAplicacion(this IServiceCollection servicios)
    {
        // Reloj del sistema (permite sustituirlo en pruebas para controlar "la hora actual").
        servicios.AddSingleton(TimeProvider.System);

        // Repositorios → SINGLETON: contienen los datos en memoria y deben vivir mientras la aplicación esté encendida.
        servicios.AddSingleton<IPacienteRepository, PacienteRepository>();
        servicios.AddSingleton<IMedicoRepository, MedicoRepository>();
        servicios.AddSingleton<ICitaRepository, CitaRepository>();

        // Servicios → SCOPED: una instancia nueva por cada petición HTTP.
        servicios.AddScoped<IPacienteService, PacienteService>();
        servicios.AddScoped<IMedicoService, MedicoService>();
        servicios.AddScoped<ICitaService, CitaService>();

        servicios.AddTransient<SembradorDeDatos>();
        return servicios;
    }
}
