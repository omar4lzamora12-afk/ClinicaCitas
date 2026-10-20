namespace ClinicaCitas.Api.Configuration;

/// <summary>
/// Parámetros de funcionamiento de la clínica. Se llena automáticamente desde la sección
/// <c>"Clinica"</c> del archivo <c>appsettings.json</c> (patrón Options de .NET), de modo que las
/// reglas de horario se pueden cambiar sin recompilar el código.
/// </summary>
public sealed class ConfiguracionClinica
{
    /// <summary>Nombre de la sección dentro de appsettings.json.</summary>
    public const string Seccion = "Clinica";

    /// <summary>Nombre comercial de la clínica.</summary>
    public string Nombre { get; set; } = "Clínica Salud Integral";

    /// <summary>Hora a la que se inicia la atención (formato hh:mm:ss).</summary>
    public TimeSpan HoraApertura { get; set; } = TimeSpan.FromHours(8);

    /// <summary>Hora a la que termina la atención (formato hh:mm:ss). Ninguna cita puede terminar después.</summary>
    public TimeSpan HoraCierre { get; set; } = TimeSpan.FromHours(18);

    /// <summary>Duración (en minutos) que se asigna a una cita cuando el cliente no la especifica.</summary>
    public int DuracionCitaPorDefectoMinutos { get; set; } = 30;

    /// <summary>Indica si la clínica atiende los domingos.</summary>
    public bool AtiendeDomingos { get; set; }

    /// <summary>Si es <c>true</c>, al iniciar se cargan pacientes, médicos y citas de ejemplo.</summary>
    public bool SembrarDatosDePrueba { get; set; } = true;
}
