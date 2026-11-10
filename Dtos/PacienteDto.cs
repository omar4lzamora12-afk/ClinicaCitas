using System.ComponentModel.DataAnnotations;

namespace ClinicaApi.Dtos
{
    public class PacienteDto
    {
        [Required(ErrorMessage = "El Nombre es Obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El Nombre debe tener entre 2 y 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Numero de DNI Invalido")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El Dni debe ser un valor numerico")]
        public string Dni { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "debe de ingresar un correo Valido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El telefono es obligatorio")]
        [RegularExpression(@"^9\d{8}$", ErrorMessage = "El telefono debe tener 9 digitos y empezar con 9")]
        public string Telefono { get; set; } = string.Empty;
    }
}
