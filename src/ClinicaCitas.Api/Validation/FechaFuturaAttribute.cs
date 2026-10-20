using System.ComponentModel.DataAnnotations;

namespace ClinicaCitas.Api.Validation;

/// <summary>
/// Validación personalizada: la fecha y hora deben ser posteriores al momento actual.
/// Un valor <c>null</c> se considera válido aquí; de exigirlo se encarga <c>[Required]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class FechaFuturaAttribute : ValidacionConRelojAttribute
{
    public FechaFuturaAttribute() : base("La fecha y hora deben ser posteriores al momento actual.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var esValida = value is DateTime fecha && fecha > Ahora(validationContext);
        return esValida
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage, new[] { validationContext.MemberName ?? string.Empty });
    }
}
