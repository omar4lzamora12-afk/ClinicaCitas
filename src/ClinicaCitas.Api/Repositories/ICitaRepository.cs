using ClinicaCitas.Api.Models.Entities;

namespace ClinicaCitas.Api.Repositories;

/// <summary>Operaciones de datos propias de las citas.</summary>
public interface ICitaRepository : IRepository<Cita>
{
    /// <summary>Todas las citas de un paciente.</summary>
    IReadOnlyList<Cita> ObtenerPorPaciente(int pacienteId);

    /// <summary>Todas las citas de un médico.</summary>
    IReadOnlyList<Cita> ObtenerPorMedico(int medicoId);

    /// <summary>¿El médico ya tiene una cita vigente que se cruza con [inicio, fin)? Se puede excluir una cita (al reprogramar).</summary>
    bool HayConflictoDeMedico(int medicoId, DateTime inicio, DateTime fin, int? excluirCitaId = null);

    /// <summary>¿El paciente ya tiene una cita vigente que se cruza con [inicio, fin)?</summary>
    bool HayConflictoDePaciente(int pacienteId, DateTime inicio, DateTime fin, int? excluirCitaId = null);
}
