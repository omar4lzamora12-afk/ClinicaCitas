using ClinicaCitas.Api.DTOs.Medicos;

namespace ClinicaCitas.Api.Services;

/// <summary>Contrato de la lógica de negocio de médicos.</summary>
public interface IMedicoService
{
    /// <summary>Lista los médicos; opcionalmente filtra por especialidad (sin distinguir mayúsculas).</summary>
    IReadOnlyList<MedicoResponseDto> ObtenerTodos(string? especialidad = null);

    /// <summary>Obtiene un médico; lanza 404 si no existe.</summary>
    MedicoResponseDto ObtenerPorId(int id);

    /// <summary>Registra un médico; lanza 409 si el DNI o la colegiatura ya existen.</summary>
    MedicoResponseDto Registrar(MedicoRequestDto solicitud);

    /// <summary>Reemplaza los datos de un médico.</summary>
    MedicoResponseDto Actualizar(int id, MedicoRequestDto solicitud);

    /// <summary>Elimina un médico; lanza 409 si tiene citas registradas.</summary>
    void Eliminar(int id);
}
