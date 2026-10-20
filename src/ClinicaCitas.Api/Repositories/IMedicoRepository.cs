using ClinicaCitas.Api.Models.Entities;

namespace ClinicaCitas.Api.Repositories;

/// <summary>Operaciones de datos propias de los médicos.</summary>
public interface IMedicoRepository : IRepository<Medico>
{
    /// <summary>Busca un médico por DNI (restricción UNIQUE simulada).</summary>
    Medico? ObtenerPorDni(string dni);

    /// <summary>Busca un médico por número de colegiatura (restricción UNIQUE simulada).</summary>
    Medico? ObtenerPorColegiatura(string numeroColegiatura);
}
