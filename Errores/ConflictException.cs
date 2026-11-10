namespace ClinicaApi.Errores
{
    // Se lanza cuando la operacion choca con una regla de negocio (duplicados, horario ocupado) -> HTTP 409
    public class ConflictException : Exception
    {
        public ConflictException(string mensaje) : base(mensaje) { }
    }
}
