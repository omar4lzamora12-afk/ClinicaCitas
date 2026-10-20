using ClinicaCitas.Api.Models.Entities;

namespace ClinicaCitas.Api.Repositories;

/// <summary>Operaciones de datos propias de los pacientes.</summary>
public interface IPacienteRepository : IRepository<Paciente>
{
    /// <summary>Busca un paciente por DNI (restricción UNIQUE simulada).</summary>
    Paciente? ObtenerPorDni(string dni);
}
