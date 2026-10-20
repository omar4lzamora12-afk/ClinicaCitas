using ClinicaCitas.Api.DTOs.Citas;
using ClinicaCitas.Api.DTOs.Medicos;
using ClinicaCitas.Api.DTOs.Pacientes;
using ClinicaCitas.Api.Models.Entities;

namespace ClinicaCitas.Api.Mappings;

/// <summary>
/// Métodos de extensión que convierten entidades en DTOs. Mantienen la conversión en un único
/// lugar y evitan depender de librerías externas como AutoMapper.
/// </summary>
public static class MapeoExtensions
{
    public static PacienteResponseDto ToResponseDto(this Paciente paciente) => new()
    {
        Id = paciente.Id,
        Nombres = paciente.Nombres,
        Apellidos = paciente.Apellidos,
        NombreCompleto = paciente.NombreCompleto,
        Dni = paciente.Dni,
        Telefono = paciente.Telefono,
        Email = paciente.Email,
        FechaNacimiento = paciente.FechaNacimiento,
        Direccion = paciente.Direccion,
        FechaCreacion = paciente.FechaCreacion,
        FechaActualizacion = paciente.FechaActualizacion
    };

    public static MedicoResponseDto ToResponseDto(this Medico medico) => new()
    {
        Id = medico.Id,
        Nombres = medico.Nombres,
        Apellidos = medico.Apellidos,
        NombreCompleto = medico.NombreCompleto,
        Dni = medico.Dni,
        Telefono = medico.Telefono,
        Email = medico.Email,
        NumeroColegiatura = medico.NumeroColegiatura,
        Especialidad = medico.Especialidad,
        Activo = medico.Activo,
        FechaCreacion = medico.FechaCreacion,
        FechaActualizacion = medico.FechaActualizacion
    };

    public static CitaResponseDto ToResponseDto(this Cita cita, Paciente paciente, Medico medico) => new()
    {
        Id = cita.Id,
        PacienteId = cita.PacienteId,
        Paciente = paciente.NombreCompleto,
        MedicoId = cita.MedicoId,
        Medico = medico.NombreCompleto,
        Especialidad = medico.Especialidad,
        FechaHora = cita.FechaHora,
        FechaFin = cita.FechaFin,
        DuracionMinutos = cita.DuracionMinutos,
        Motivo = cita.Motivo,
        Observaciones = cita.Observaciones,
        Estado = cita.Estado,
        MotivoCancelacion = cita.MotivoCancelacion,
        FechaCreacion = cita.FechaCreacion,
        FechaActualizacion = cita.FechaActualizacion
    };
}
