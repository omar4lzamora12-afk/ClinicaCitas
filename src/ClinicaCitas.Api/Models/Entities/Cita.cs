using ClinicaCitas.Api.Exceptions;
using ClinicaCitas.Api.Models.Enums;

namespace ClinicaCitas.Api.Models.Entities;

/// <summary>
/// Cita médica que vincula a un <see cref="Paciente"/> con un <see cref="Medico"/> en un horario.
/// ENCAPSULAMIENTO: todos los setters son privados; el estado solo cambia mediante los métodos
/// de la clase (<see cref="Confirmar"/>, <see cref="Cancelar"/>, etc.), que validan que la
/// transición sea legal. Así es imposible dejar una cita en un estado inconsistente.
/// </summary>
public sealed class Cita : EntidadBase
{
    /// <summary>Id del paciente (relación tipo clave foránea).</summary>
    public int PacienteId { get; private set; }

    /// <summary>Id del médico (relación tipo clave foránea).</summary>
    public int MedicoId { get; private set; }

    /// <summary>Fecha y hora de inicio, en hora local de la clínica.</summary>
    public DateTime FechaHora { get; private set; }

    /// <summary>Duración en minutos.</summary>
    public int DuracionMinutos { get; private set; }

    /// <summary>Motivo de la consulta.</summary>
    public string Motivo { get; private set; } = string.Empty;

    /// <summary>Notas adicionales (opcional).</summary>
    public string? Observaciones { get; private set; }

    /// <summary>Estado actual dentro del ciclo de vida.</summary>
    public EstadoCita Estado { get; private set; }

    /// <summary>Razón de la cancelación; solo tiene valor si <see cref="Estado"/> es Cancelada.</summary>
    public string? MotivoCancelacion { get; private set; }

    /// <summary>Hora de finalización calculada (inicio + duración).</summary>
    public DateTime FechaFin => FechaHora.AddMinutes(DuracionMinutos);

    /// <summary><c>true</c> si la cita sigue vigente (Programada o Confirmada).</summary>
    public bool EstaActiva => Estado is EstadoCita.Programada or EstadoCita.Confirmada;

    public Cita(int pacienteId, int medicoId, DateTime fechaHora, int duracionMinutos,
                string motivo, string? observaciones = null)
    {
        PacienteId = pacienteId;
        AsignarDatosDeLaConsulta(medicoId, fechaHora, duracionMinutos, motivo, observaciones);
        Estado = EstadoCita.Programada;
    }

    /// <summary>Indica si el intervalo [inicio, fin) se cruza con el horario de esta cita.</summary>
    public bool SeSuperponeCon(DateTime inicio, DateTime fin) => FechaHora < fin && inicio < FechaFin;

    /// <summary>Cambia médico, horario y datos de la consulta. La cita vuelve a quedar Programada.</summary>
    public void Reprogramar(int medicoId, DateTime fechaHora, int duracionMinutos, string motivo, string? observaciones)
    {
        ExigirEstadoActivo("modificar");
        AsignarDatosDeLaConsulta(medicoId, fechaHora, duracionMinutos, motivo, observaciones);
        Estado = EstadoCita.Programada;
    }

    /// <summary>Programada → Confirmada.</summary>
    public void Confirmar()
    {
        if (Estado != EstadoCita.Programada)
        {
            throw new ConflictoException(
                $"Solo se puede confirmar una cita en estado '{EstadoCita.Programada}'. Estado actual: '{Estado}'.");
        }
        Estado = EstadoCita.Confirmada;
    }

    /// <summary>Programada o Confirmada → Atendida.</summary>
    public void Atender()
    {
        ExigirEstadoActivo("marcar como atendida");
        Estado = EstadoCita.Atendida;
    }

    /// <summary>Programada o Confirmada → Cancelada (conserva el motivo).</summary>
    public void Cancelar(string motivoCancelacion)
    {
        ExigirEstadoActivo("cancelar");
        MotivoCancelacion = LimpiarObligatorio(motivoCancelacion, nameof(motivoCancelacion));
        Estado = EstadoCita.Cancelada;
    }

    private void AsignarDatosDeLaConsulta(int medicoId, DateTime fechaHora, int duracionMinutos,
                                          string motivo, string? observaciones)
    {
        MedicoId = medicoId;
        FechaHora = fechaHora;
        DuracionMinutos = duracionMinutos;
        Motivo = LimpiarObligatorio(motivo, nameof(motivo));
        Observaciones = LimpiarOpcional(observaciones);
    }

    private void ExigirEstadoActivo(string accion)
    {
        if (!EstaActiva)
        {
            throw new ConflictoException(
                $"No se puede {accion} una cita en estado '{Estado}'. Solo las citas Programadas o Confirmadas admiten cambios.");
        }
    }
}
