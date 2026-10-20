using ClinicaCitas.Api.Models.Entities;

namespace ClinicaCitas.Api.Repositories;

/// <summary>
/// Simulación de una tabla de base de datos usando un mapa <c>Dictionary&lt;int, TEntidad&gt;</c>
/// (clave primaria → fila). Imita el comportamiento de una BD real:
/// <list type="bullet">
///   <item><description>IDs autoincrementales que nunca se reutilizan (como una columna IDENTITY).</description></item>
///   <item><description>Entrega y guarda COPIAS de las filas: modificar un objeto no altera la "tabla" hasta llamar a <see cref="Actualizar"/>.</description></item>
///   <item><description><c>lock</c> para que peticiones simultáneas no corrompan los datos.</description></item>
///   <item><description>Campos de auditoría (creación/actualización) asignados por la capa de datos.</description></item>
/// </list>
/// </summary>
public class InMemoryRepository<TEntidad> : IRepository<TEntidad> where TEntidad : EntidadBase
{
    private readonly Dictionary<int, TEntidad> _tabla = new();
    private readonly object _bloqueo = new();
    private readonly TimeProvider _reloj;
    private int _ultimoId;

    protected InMemoryRepository(TimeProvider reloj) => _reloj = reloj;

    public IReadOnlyList<TEntidad> ObtenerTodos()
    {
        lock (_bloqueo)
        {
            return _tabla.Values.OrderBy(e => e.Id).Select(Copiar).ToList();
        }
    }

    public TEntidad? ObtenerPorId(int id)
    {
        lock (_bloqueo)
        {
            return _tabla.TryGetValue(id, out var entidad) ? Copiar(entidad) : null;
        }
    }

    public IReadOnlyList<TEntidad> Buscar(Func<TEntidad, bool> criterio)
    {
        lock (_bloqueo)
        {
            return _tabla.Values.Where(criterio).OrderBy(e => e.Id).Select(Copiar).ToList();
        }
    }

    public int Contar(Func<TEntidad, bool>? criterio = null)
    {
        lock (_bloqueo)
        {
            return criterio is null ? _tabla.Count : _tabla.Values.Count(criterio);
        }
    }

    public TEntidad Agregar(TEntidad entidad)
    {
        lock (_bloqueo)
        {
            var fila = Copiar(entidad);
            fila.Id = ++_ultimoId;
            fila.RegistrarCreacion(Ahora());
            _tabla[fila.Id] = fila;
            return Copiar(fila);
        }
    }

    public TEntidad? Actualizar(TEntidad entidad)
    {
        lock (_bloqueo)
        {
            if (!_tabla.ContainsKey(entidad.Id))
            {
                return null;
            }

            var fila = Copiar(entidad);
            fila.RegistrarActualizacion(Ahora());
            _tabla[fila.Id] = fila;
            return Copiar(fila);
        }
    }

    public bool Eliminar(int id)
    {
        lock (_bloqueo)
        {
            return _tabla.Remove(id);
        }
    }

    private DateTime Ahora() => _reloj.GetLocalNow().DateTime;

    private static TEntidad Copiar(TEntidad entidad) => (TEntidad)entidad.CrearCopia();
}
