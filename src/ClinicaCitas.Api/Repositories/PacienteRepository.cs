using ClinicaCitas.Api.Models.Entities;

namespace ClinicaCitas.Api.Repositories;

/// <summary>Tabla simulada de pacientes. Reutiliza todo el CRUD de <see cref="InMemoryRepository{TEntidad}"/>.</summary>
public sealed class PacienteRepository : InMemoryRepository<Paciente>, IPacienteRepository
{
    public PacienteRepository(TimeProvider reloj) : base(reloj)
    {
    }

    public Paciente? ObtenerPorDni(string dni) => Buscar(p => p.Dni == dni).FirstOrDefault();
}
