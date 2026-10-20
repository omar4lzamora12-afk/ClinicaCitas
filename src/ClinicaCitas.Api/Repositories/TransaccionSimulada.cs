namespace ClinicaCitas.Api.Repositories;

/// <summary>
/// Simula una transacción de base de datos con aislamiento serializable: mientras un servicio ejecuta
/// "verificar + guardar", ninguna otra petición puede hacer lo mismo. Evita, por ejemplo, que dos
/// personas reserven el mismo horario del mismo médico en el mismo instante.
/// </summary>
public static class TransaccionSimulada
{
    private static readonly object Bloqueo = new();

    /// <summary>Ejecuta la operación de forma exclusiva y devuelve su resultado.</summary>
    public static T Ejecutar<T>(Func<T> operacion)
    {
        lock (Bloqueo)
        {
            return operacion();
        }
    }

    /// <summary>Ejecuta la operación de forma exclusiva.</summary>
    public static void Ejecutar(Action operacion)
    {
        lock (Bloqueo)
        {
            operacion();
        }
    }
}
