namespace ClinicaApi.Errores
{
    // Se lanza cuando un recurso (cita, paciente, medico) no existe -> HTTP 404
    public class NotFoundException : Exception
    {
        public NotFoundException(string mensaje) : base(mensaje) { }
    }
}
