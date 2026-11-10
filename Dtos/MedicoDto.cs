using System.ComponentModel.DataAnnotations;

namespace ClinicaApi.Dtos
{
    public class MedicoDto
    {
        [Required(ErrorMessage = "El Nombre es Obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El Nombre debe tener entre 2 y 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Especialidad es Obligatoria")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "La Especialidad debe tener entre 3 y 50 caracteres")]
        public string Especialidad { get; set; } = string.Empty;
    }
}
