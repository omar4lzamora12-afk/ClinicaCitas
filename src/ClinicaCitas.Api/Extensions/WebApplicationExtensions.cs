using ClinicaCitas.Api.Configuration;
using ClinicaCitas.Api.Data;
using Microsoft.Extensions.Options;

namespace ClinicaCitas.Api.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>Carga los datos de ejemplo si así lo indica <c>Clinica:SembrarDatosDePrueba</c> en appsettings.json.</summary>
    public static WebApplication SembrarDatosSiCorresponde(this WebApplication app)
    {
        var configuracion = app.Services.GetRequiredService<IOptions<ConfiguracionClinica>>().Value;

        if (configuracion.SembrarDatosDePrueba)
        {
            using var alcance = app.Services.CreateScope();
            alcance.ServiceProvider.GetRequiredService<SembradorDeDatos>().Sembrar();
        }

        return app;
    }
}
