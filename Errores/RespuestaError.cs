namespace ClinicaApi.Errores
{
    // Clase sencilla para darle formato JSON uniforme a las respuestas de error
    public class RespuestaError
    {
        public int CodigoEstado { get; set; } // Ejemplo: 404
        public string Mensaje { get; set; } = string.Empty; // Mensaje de error para el usuario
        public string Ruta { get; set; } = string.Empty; // Endpoint donde ocurrió el error
    }
}
