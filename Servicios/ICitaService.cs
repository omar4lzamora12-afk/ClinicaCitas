using ClinicaApi.Dtos;
using ClinicaApi.Entidades;

namespace ClinicaApi.Servicios
{
    public interface ICitaService
    {
        List<Cita> ObtenerTodas();
        Cita ObtenerPorId(int id);
        Cita Registrar(CitaDto dto);
        Cita Actualizar(int id, CitaDto dto);
        Cita Cancelar(int id);
        void Eliminar(int id);
    }
}
