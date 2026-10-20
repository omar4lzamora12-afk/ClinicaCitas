namespace ClinicaCitas.Api.Models.Entities;

/// <summary>
/// Clase abstracta con los datos comunes de las personas del sistema (pacientes y médicos).
/// Aplica ABSTRACCIÓN (no se puede instanciar) y HERENCIA (la reutilizan <see cref="Paciente"/> y <see cref="Medico"/>).
/// </summary>
public abstract class Persona : EntidadBase
{
    /// <summary>Nombres de pila.</summary>
    public string Nombres { get; private set; } = string.Empty;

    /// <summary>Apellidos.</summary>
    public string Apellidos { get; private set; } = string.Empty;

    /// <summary>Documento Nacional de Identidad (8 dígitos).</summary>
    public string Dni { get; private set; } = string.Empty;

    /// <summary>Teléfono celular de contacto.</summary>
    public string Telefono { get; private set; } = string.Empty;

    /// <summary>Correo electrónico (se guarda en minúsculas).</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Nombre completo calculado; no se almacena.</summary>
    public string NombreCompleto => $"{Nombres} {Apellidos}";

    protected Persona(string nombres, string apellidos, string dni, string telefono, string email) =>
        ActualizarDatosPersonales(nombres, apellidos, dni, telefono, email);

    /// <summary>Modifica los datos comunes. Es <c>protected</c>: solo las clases hijas deciden cuándo exponerlo.</summary>
    protected void ActualizarDatosPersonales(string nombres, string apellidos, string dni, string telefono, string email)
    {
        Nombres = LimpiarObligatorio(nombres, nameof(nombres));
        Apellidos = LimpiarObligatorio(apellidos, nameof(apellidos));
        Dni = LimpiarObligatorio(dni, nameof(dni));
        Telefono = LimpiarObligatorio(telefono, nameof(telefono));
        Email = LimpiarObligatorio(email, nameof(email)).ToLowerInvariant();
    }
}
