using ClinicaApi.Dtos;
using ClinicaApi.Entidades;

namespace ClinicaApi.Servicios
{
    public interface IPacienteService
    {
        List<Paciente> ObtenerTodos();
        Paciente ObtenerPorId(int id);
        Paciente Registrar(PacienteDto dto);
    }
}
