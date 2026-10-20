namespace ClinicaCitas.Api.Models.Enums;

/// <summary>
/// Ciclo de vida de una cita médica.
/// <code>
/// Programada ──► Confirmada ──► Atendida
///     │              │
///     └──────────────┴──────► Cancelada
/// </code>
/// <c>Atendida</c> y <c>Cancelada</c> son estados finales: ya no admiten cambios.
/// </summary>
public enum EstadoCita
{
    /// <summary>Cita registrada, pendiente de confirmación.</summary>
    Programada = 1,

    /// <summary>El paciente confirmó su asistencia.</summary>
    Confirmada = 2,

    /// <summary>El paciente fue atendido (estado final).</summary>
    Atendida = 3,

    /// <summary>La cita fue cancelada (estado final).</summary>
    Cancelada = 4
}
