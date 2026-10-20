using System.ComponentModel.DataAnnotations;
using ClinicaCitas.Api.Validation;

namespace ClinicaCitas.Api.DTOs.Pacientes;

/// <summary>Datos que se envían para registrar (POST) o reemplazar (PUT) un paciente.</summary>
public sealed class PacienteRequestDto
{
    /// <summary>Nombres de pila. Ejemplo: "Lucía Fernanda".</summary>
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 80 caracteres.")]
    [RegularExpression(PatronesValidacion.SoloLetras, ErrorMessage = "Los nombres solo pueden contener letras y espacios.")]
    public string Nombres { get; init; } = string.Empty;

    /// <summary>Apellidos. Ejemplo: "Fernández Rojas".</summary>
    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 80 caracteres.")]
    [RegularExpression(PatronesValidacion.SoloLetras, ErrorMessage = "Los apellidos solo pueden contener letras y espacios.")]
    public string Apellidos { get; init; } = string.Empty;

    /// <summary>DNI de 8 dígitos. Debe ser único entre pacientes.</summary>
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

    /// <summary>Fecha de nacimiento en formato AAAA-MM-DD.</summary>
    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [FechaNoFutura(ErrorMessage = "La fecha de nacimiento debe estar entre el 01/01/1900 y la fecha de hoy.")]
    public DateOnly? FechaNacimiento { get; init; }

    /// <summary>Dirección de domicilio (opcional).</summary>
    [StringLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres.")]
    public string? Direccion { get; init; }
}
