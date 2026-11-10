using ClinicaApi.Dtos;
using ClinicaApi.Entidades;

namespace ClinicaApi.Servicios
{
    public interface IMedicoService
    {
        List<Medico> ObtenerTodos();
        Medico ObtenerPorId(int id);
        Medico Registrar(MedicoDto dto);
    }
}
