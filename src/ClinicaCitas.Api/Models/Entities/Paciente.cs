namespace ClinicaCitas.Api.Models.Entities;

/// <summary>Persona que solicita atención médica en la clínica.</summary>
public sealed class Paciente : Persona
{
    /// <summary>Fecha de nacimiento.</summary>
    public DateOnly FechaNacimiento { get; private set; }

    /// <summary>Dirección de domicilio (opcional).</summary>
    public string? Direccion { get; private set; }

    public Paciente(string nombres, string apellidos, string dni, string telefono, string email,
                    DateOnly fechaNacimiento, string? direccion = null)
        : base(nombres, apellidos, dni, telefono, email)
    {
        FechaNacimiento = fechaNacimiento;
        Direccion = LimpiarOpcional(direccion);
    }

    /// <summary>Reemplaza los datos editables del paciente.</summary>
    public void Actualizar(string nombres, string apellidos, string dni, string telefono, string email,
                           DateOnly fechaNacimiento, string? direccion)
    {
        ActualizarDatosPersonales(nombres, apellidos, dni, telefono, email);
        FechaNacimiento = fechaNacimiento;
        Direccion = LimpiarOpcional(direccion);
    }
}
