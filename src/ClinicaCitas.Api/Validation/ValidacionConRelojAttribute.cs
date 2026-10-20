using System.ComponentModel.DataAnnotations;

namespace ClinicaCitas.Api.Validation;

/// <summary>
/// Clase base de las validaciones que dependen de "la fecha de hoy". Obtiene el reloj desde la
/// inyección de dependencias (<see cref="TimeProvider"/>), lo que evita depender de <c>DateTime.Now</c>.
/// </summary>
public abstract class ValidacionConRelojAttribute : ValidationAttribute
{
    protected ValidacionConRelojAttribute(string mensajePorDefecto) : base(mensajePorDefecto)
    {
    }

    /// <summary>Hora local actual según el reloj registrado en el contenedor de dependencias.</summary>
    protected static DateTime Ahora(ValidationContext contexto)
    {
        var reloj = contexto.GetService(typeof(TimeProvider)) as TimeProvider ?? TimeProvider.System;
        return reloj.GetLocalNow().DateTime;
    }
}
