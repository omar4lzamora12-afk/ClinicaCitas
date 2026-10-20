using ClinicaCitas.Api.DTOs.Pacientes;

namespace ClinicaCitas.Api.Services;

/// <summary>Contrato de la lógica de negocio de pacientes. Los controladores solo conocen esta interfaz.</summary>
public interface IPacienteService
{
    /// <summary>Lista todos los pacientes.</summary>
    IReadOnlyList<PacienteResponseDto> ObtenerTodos();

    /// <summary>Obtiene un paciente; lanza 404 si no existe.</summary>
    PacienteResponseDto ObtenerPorId(int id);

    /// <summary>Registra un paciente; lanza 409 si el DNI ya existe.</summary>
    PacienteResponseDto Registrar(PacienteRequestDto solicitud);

    /// <summary>Reemplaza los datos de un paciente; lanza 404 o 409 según corresponda.</summary>
    PacienteResponseDto Actualizar(int id, PacienteRequestDto solicitud);

    /// <summary>Elimina un paciente; lanza 409 si tiene citas registradas.</summary>
    void Eliminar(int id);
}
