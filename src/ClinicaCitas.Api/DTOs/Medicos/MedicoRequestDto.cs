using System.ComponentModel.DataAnnotations;
using ClinicaCitas.Api.Validation;

namespace ClinicaCitas.Api.DTOs.Medicos;

/// <summary>Datos que se envían para registrar (POST) o reemplazar (PUT) un médico.</summary>
public sealed class MedicoRequestDto
{
    /// <summary>Nombres de pila.</summary>
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 80 caracteres.")]
    [RegularExpression(PatronesValidacion.SoloLetras, ErrorMessage = "Los nombres solo pueden contener letras y espacios.")]
    public string Nombres { get; init; } = string.Empty;

    /// <summary>Apellidos.</summary>
    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 80 caracteres.")]
    [RegularExpression(PatronesValidacion.SoloLetras, ErrorMessage = "Los apellidos solo pueden contener letras y espacios.")]
    public string Apellidos { get; init; } = string.Empty;

    /// <summary>DNI de 8 dígitos. Debe ser único entre médicos.</summary>
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(PatronesValidacion.Dni, ErrorMessage = "El DNI debe tener exactamente 8 dígitos numéricos.")]
    public string Dni { get; init; } = string.Empty;

    /// <summary>Celular de 9 dígitos que empiece con 9.</summary>
    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(PatronesValidacion.CelularPeru, ErrorMessage = "El teléfono debe ser un celular de 9 dígitos que empiece con 9.")]
    public string Telefono { get; init; } = string.Empty;

    /// <summary>Correo electrónico válido.</summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
    public string Email { get; init; } = string.Empty;

    /// <summary>Número de colegiatura (CMP) de 5 o 6 dígitos. Debe ser único.</summary>
    [Required(ErrorMessage = "El número de colegiatura es obligatorio.")]
    [RegularExpression(PatronesValidacion.Colegiatura, ErrorMessage = "El número de colegiatura debe tener 5 o 6 dígitos.")]
    public string NumeroColegiatura { get; init; } = string.Empty;

    /// <summary>Especialidad médica. Ejemplo: "Pediatría".</summary>
    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "La especialidad debe tener entre 3 y 60 caracteres.")]
    public string Especialidad { get; init; } = string.Empty;

    /// <summary>Si es <c>false</c>, el médico no puede recibir nuevas citas. Por defecto: <c>true</c>.</summary>
    public bool Activo { get; init; } = true;
}
