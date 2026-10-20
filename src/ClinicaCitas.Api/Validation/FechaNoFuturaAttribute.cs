using System.ComponentModel.DataAnnotations;

namespace ClinicaCitas.Api.Validation;

/// <summary>
/// Validación personalizada para fechas de nacimiento (<see cref="DateOnly"/>): entre el 01/01/1900 y hoy.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class FechaNoFuturaAttribute : ValidacionConRelojAttribute
{
    private static readonly DateOnly LimiteInferior = new(1900, 1, 1);

    public FechaNoFuturaAttribute() : base("La fecha debe estar entre el 01/01/1900 y la fecha de hoy.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var hoy = DateOnly.FromDateTime(Ahora(validationContext));
        var esValida = value is DateOnly fecha && fecha >= LimiteInferior && fecha <= hoy;
        return esValida
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage, new[] { validationContext.MemberName ?? string.Empty });
    }
}
