using ClinicaCitas.Api.Models.Entities;

namespace ClinicaCitas.Api.Repositories;

/// <summary>
/// Contrato genérico de acceso a datos (ABSTRACCIÓN). Los servicios dependen de esta interfaz y no de
/// cómo se guardan los datos: hoy es memoria, mañana podría ser SQL Server sin tocar la lógica de negocio.
/// </summary>
/// <typeparam name="TEntidad">Tipo de entidad que administra el repositorio.</typeparam>
public interface IRepository<TEntidad> where TEntidad : EntidadBase
{
    /// <summary>Devuelve todos los registros ordenados por Id.</summary>
    IReadOnlyList<TEntidad> ObtenerTodos();

    /// <summary>Busca por clave primaria; <c>null</c> si no existe.</summary>
    TEntidad? ObtenerPorId(int id);

    /// <summary>Devuelve los registros que cumplen el criterio.</summary>
    IReadOnlyList<TEntidad> Buscar(Func<TEntidad, bool> criterio);

    /// <summary>Cuenta los registros (todos, o solo los que cumplen el criterio).</summary>
    int Contar(Func<TEntidad, bool>? criterio = null);

    /// <summary>Inserta un registro nuevo, le asigna Id autoincremental y devuelve lo guardado.</summary>
    TEntidad Agregar(TEntidad entidad);

    /// <summary>Guarda los cambios de un registro existente; <c>null</c> si ya no existe.</summary>
    TEntidad? Actualizar(TEntidad entidad);

    /// <summary>Elimina el registro; <c>false</c> si no existía.</summary>
    bool Eliminar(int id);
}
