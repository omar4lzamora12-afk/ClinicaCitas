using ClinicaCitas.Api.Models.Entities;

namespace ClinicaCitas.Api.Repositories;

/// <summary>Tabla simulada de médicos.</summary>
public sealed class MedicoRepository : InMemoryRepository<Medico>, IMedicoRepository
{
    public MedicoRepository(TimeProvider reloj) : base(reloj)
    {
    }

    public Medico? ObtenerPorDni(string dni) => Buscar(m => m.Dni == dni).FirstOrDefault();

    public Medico? ObtenerPorColegiatura(string numeroColegiatura) =>
        Buscar(m => m.NumeroColegiatura == numeroColegiatura).FirstOrDefault();
}
