namespace ClinicaCitas.Api.Models.Entities;

/// <summary>Profesional de la salud que atiende las citas.</summary>
public sealed class Medico : Persona
{
    /// <summary>Número de colegiatura del Colegio Médico del Perú (CMP).</summary>
    public string NumeroColegiatura { get; private set; } = string.Empty;

    /// <summary>Especialidad médica (por ejemplo, "Pediatría").</summary>
    public string Especialidad { get; private set; } = string.Empty;

    /// <summary>Un médico inactivo no puede recibir nuevas citas.</summary>
    public bool Activo { get; private set; }

    public Medico(string nombres, string apellidos, string dni, string telefono, string email,
                  string numeroColegiatura, string especialidad, bool activo = true)
        : base(nombres, apellidos, dni, telefono, email)
    {
        NumeroColegiatura = LimpiarObligatorio(numeroColegiatura, nameof(numeroColegiatura));
        Especialidad = LimpiarObligatorio(especialidad, nameof(especialidad));
        Activo = activo;
    }

    /// <summary>Reemplaza los datos editables del médico.</summary>
    public void Actualizar(string nombres, string apellidos, string dni, string telefono, string email,
                           string numeroColegiatura, string especialidad, bool activo)
    {
        ActualizarDatosPersonales(nombres, apellidos, dni, telefono, email);
        NumeroColegiatura = LimpiarObligatorio(numeroColegiatura, nameof(numeroColegiatura));
        Especialidad = LimpiarObligatorio(especialidad, nameof(especialidad));
        Activo = activo;
    }
}
