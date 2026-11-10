using ClinicaApi.Dtos;
using ClinicaApi.Entidades;
using ClinicaApi.Errores;

namespace ClinicaApi.Servicios
{
    // SRP: contiene la regla de negocio de las citas (validar y coordinar).
    // DIP: pide los servicios de paciente y médico desde afuera en el constructor (Inyección de dependencias).
    public class CitaService(IPacienteService pacienteService, IMedicoService medicoService) : ICitaService
    {
        private readonly List<Cita> _citas = new(); // Simula la base de datos en memoria
        private int _siguienteId = 1;

        public List<Cita> ObtenerTodas()
        {
            return _citas.ToList();
        }

        public Cita ObtenerPorId(int id)
        {
            var cita = _citas.FirstOrDefault(x => x.Id == id);
            if (cita == null)
            {
                throw new NotFoundException($"El codigo de Cita: {id} ingresado no Existe, Verifique!!");
            }
            return cita;
        }

        public Cita Registrar(CitaDto dto)
        {
            ValidarReglas(dto, null);

            var nuevaCita = new Cita // Mapeo campos de la entidad con Entidad DTO
            {
                Id = _siguienteId++,
                PacienteId = dto.PacienteId,
                MedicoId = dto.MedicoId,
                FechaHora = dto.FechaHora!.Value,
                Motivo = dto.Motivo,
                Estado = "Programada"
            };

            _citas.Add(nuevaCita);
            return nuevaCita;
        }

        public Cita Actualizar(int id, CitaDto dto)
        {
            var cita = ObtenerPorId(id);

            if (cita.Estado == "Cancelada")
            {
                throw new ConflictException($"La cita: {id} esta cancelada y no se puede modificar.");
            }

            ValidarReglas(dto, id);

            cita.PacienteId = dto.PacienteId;
            cita.MedicoId = dto.MedicoId;
            cita.FechaHora = dto.FechaHora!.Value;
            cita.Motivo = dto.Motivo;
            return cita;
        }

        public Cita Cancelar(int id)
        {
            var cita = ObtenerPorId(id);

            if (cita.Estado == "Cancelada")
            {
                throw new ConflictException($"La cita: {id} ya se encuentra cancelada.");
            }

            cita.Estado = "Cancelada";
            return cita;
        }

        public void Eliminar(int id)
        {
            var cita = ObtenerPorId(id);
            _citas.Remove(cita);
        }

        // Reglas de negocio comunes para registrar y actualizar una cita
        private void ValidarReglas(CitaDto dto, int? idActual)
        {
            pacienteService.ObtenerPorId(dto.PacienteId); // lanza NotFoundException si el paciente no existe
            medicoService.ObtenerPorId(dto.MedicoId);     // lanza NotFoundException si el médico no existe

            var fecha = dto.FechaHora!.Value;

            if (fecha <= DateTime.Now)
            {
                throw new ArgumentException("La fecha y hora de la cita debe ser posterior a la fecha actual.");
            }

            // El mismo médico no puede tener dos citas programadas a la misma fecha y hora
            var horarioOcupado = _citas.Any(x => x.MedicoId == dto.MedicoId
                                              && x.FechaHora == fecha
                                              && x.Estado == "Programada"
                                              && x.Id != idActual);
            if (horarioOcupado)
            {
                throw new ConflictException($"El medico {dto.MedicoId} ya tiene una cita programada en ese horario.");
            }
        }
    }
}
