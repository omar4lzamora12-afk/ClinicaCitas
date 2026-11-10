using System.ComponentModel.DataAnnotations;

namespace ClinicaApi.Dtos
{
    public class CitaDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un PacienteId valido")]
        public int PacienteId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un MedicoId valido")]
        public int MedicoId { get; set; }

        // DateTime? para que [Required] funcione cuando el cliente no envia la fecha
        [Required(ErrorMessage = "La fecha y hora de la cita es obligatoria")]
        public DateTime? FechaHora { get; set; }

        [Required(ErrorMessage = "El motivo de la cita es obligatorio")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "El Motivo debe tener entre 5 y 200 caracteres")]
        public string Motivo { get; set; } = string.Empty;
    }
}
