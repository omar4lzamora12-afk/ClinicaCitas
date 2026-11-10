using ClinicaApi.Dtos;
using ClinicaApi.Entidades;
using ClinicaApi.Errores;

namespace ClinicaApi.Servicios
{
    // SRP: su única responsabilidad es gestionar los pacientes en memoria
    public class PacienteService : IPacienteService
    {
        private readonly List<Paciente> _pacientes = new()
        {
            new Paciente { Id = 1, Nombre = "Maria Lopez",  Dni = "12345678", Email = "maria.lopez@correo.com",  Telefono = "987654321" },
            new Paciente { Id = 2, Nombre = "Carlos Perez", Dni = "87654321", Email = "carlos.perez@correo.com", Telefono = "912345678" }
        };

        private int _siguienteId = 3;

        public List<Paciente> ObtenerTodos()
        {
            return _pacientes.ToList();
        }

        public Paciente ObtenerPorId(int id)
        {
            var paciente = _pacientes.FirstOrDefault(x => x.Id == id);
            if (paciente == null)
            {
                throw new NotFoundException($"El codigo de Paciente: {id} ingresado no Existe, Verifique!!");
            }
            return paciente;
        }

        public Paciente Registrar(PacienteDto dto)
        {
            var existePaciente = _pacientes.Any(x => x.Dni == dto.Dni); // valida duplicado por DNI
            if (existePaciente)
            {
                throw new ConflictException($"Ya existe un paciente con el DNI: {dto.Dni}, Verifique!!");
            }

            var nuevoPaciente = new Paciente // Mapeo campos de la entidad con Entidad DTO
            {
                Id = _siguienteId++,
                Nombre = dto.Nombre,
                Dni = dto.Dni,
                Email = dto.Email,
                Telefono = dto.Telefono
            };

            _pacientes.Add(nuevoPaciente);
            return nuevoPaciente;
        }
    }
}
