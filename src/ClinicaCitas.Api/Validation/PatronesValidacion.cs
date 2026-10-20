namespace ClinicaCitas.Api.Validation;

/// <summary>Expresiones regulares reutilizadas por las anotaciones <c>[RegularExpression]</c> de los DTOs.</summary>
public static class PatronesValidacion
{
    /// <summary>Letras (con tildes y ñ), espacios, apóstrofes, puntos y guiones. No admite números.</summary>
    public const string SoloLetras = @"^[\p{L}][\p{L}\s'.\-]*$";

    /// <summary>DNI peruano: exactamente 8 dígitos.</summary>
    public const string Dni = @"^[0-9]{8}$";

    /// <summary>Celular peruano: 9 dígitos que empiezan con 9.</summary>
    public const string CelularPeru = @"^9[0-9]{8}$";

    /// <summary>Número de colegiatura (CMP): 5 o 6 dígitos.</summary>
    public const string Colegiatura = @"^[0-9]{5,6}$";
}
