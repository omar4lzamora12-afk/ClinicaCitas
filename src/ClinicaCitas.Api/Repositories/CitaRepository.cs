using ClinicaCitas.Api.Models.Entities;

namespace ClinicaCitas.Api.Repositories;

/// <summary>Tabla simulada de citas (el equivalente a <c>List&lt;Cita&gt;</c> del enunciado, indexada por Id).</summary>
public sealed class CitaRepository : InMemoryRepository<Cita>, ICitaRepository
{
    public CitaRepository(TimeProvider reloj) : base(reloj)
    {
    }

    public IReadOnlyList<Cita> ObtenerPorPaciente(int pacienteId) => Buscar(c => c.PacienteId == pacienteId);

    public IReadOnlyList<Cita> ObtenerPorMedico(int medicoId) => Buscar(c => c.MedicoId == medicoId);

    public bool HayConflictoDeMedico(int medicoId, DateTime inicio, DateTime fin, int? excluirCitaId = null) =>
        Contar(c => c.EstaActiva && c.MedicoId == medicoId && c.Id != excluirCitaId && c.SeSuperponeCon(inicio, fin)) > 0;

    public bool HayConflictoDePaciente(int pacienteId, DateTime inicio, DateTime fin, int? excluirCitaId = null) =>
        Contar(c => c.EstaActiva && c.PacienteId == pacienteId && c.Id != excluirCitaId && c.SeSuperponeCon(inicio, fin)) > 0;
}
