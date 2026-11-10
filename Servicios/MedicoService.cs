using ClinicaApi.Dtos;
using ClinicaApi.Entidades;
using ClinicaApi.Errores;

namespace ClinicaApi.Servicios
{
    // SRP: su única responsabilidad es gestionar los médicos en memoria
    public class MedicoService : IMedicoService
    {
        private readonly List<Medico> _medicos = new()
        {
            new Medico { Id = 1, Nombre = "Dr. Juan Ramirez",  Especialidad = "Cardiologia" },
            new Medico { Id = 2, Nombre = "Dra. Lucia Torres", Especialidad = "Pediatria" },
            new Medico { Id = 3, Nombre = "Dr. Pedro Salazar", Especialidad = "Medicina General" }
        };

        private int _siguienteId = 4;

        public List<Medico> ObtenerTodos()
        {
            return _medicos.ToList();
        }

        public Medico ObtenerPorId(int id)
        {
            var medico = _medicos.FirstOrDefault(x => x.Id == id);
            if (medico == null)
            {
                throw new NotFoundException($"El codigo de Medico: {id} ingresado no Existe, Verifique!!");
            }
            return medico;
        }

        public Medico Registrar(MedicoDto dto)
        {
            var nuevoMedico = new Medico // Mapeo campos de la entidad con Entidad DTO
            {
                Id = _siguienteId++,
                Nombre = dto.Nombre,
                Especialidad = dto.Especialidad
            };

            _medicos.Add(nuevoMedico);
            return nuevoMedico;
        }
    }
}
