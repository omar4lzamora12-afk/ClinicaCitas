namespace ClinicaCitas.Api.Models.Entities;

/// <summary>
/// Clase base abstracta de todas las entidades. Centraliza la identidad (<see cref="Id"/>) y los
/// campos de auditoría, y ofrece utilidades de limpieza de texto reutilizables por las clases hijas
/// (REUTILIZACIÓN mediante HERENCIA).
/// </summary>
public abstract class EntidadBase
{
    /// <summary>Identificador único. Solo la capa de persistencia puede asignarlo (setter <c>internal</c>).</summary>
    public int Id { get; internal set; }

    /// <summary>Fecha y hora en que se registró la entidad (la asigna el repositorio, como un DEFAULT de BD).</summary>
    public DateTime FechaCreacion { get; private set; }

    /// <summary>Fecha y hora de la última modificación; <c>null</c> si nunca se modificó.</summary>
    public DateTime? FechaActualizacion { get; private set; }

    internal void RegistrarCreacion(DateTime ahora) => FechaCreacion = ahora;

    internal void RegistrarActualizacion(DateTime ahora) => FechaActualizacion = ahora;

    /// <summary>Crea una copia superficial. El repositorio la usa para entregar "filas" independientes, como haría una BD.</summary>
    internal EntidadBase CrearCopia() => (EntidadBase)MemberwiseClone();

    /// <summary>Quita espacios sobrantes y rechaza valores vacíos (protege las invariantes de la entidad).</summary>
    protected static string LimpiarObligatorio(string? valor, string nombreParametro)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valor, nombreParametro);
        return valor.Trim();
    }

    /// <summary>Quita espacios sobrantes; devuelve <c>null</c> si el texto está vacío.</summary>
    protected static string? LimpiarOpcional(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
