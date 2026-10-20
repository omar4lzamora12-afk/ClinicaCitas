using ClinicaCitas.Api.DTOs.Citas;
using ClinicaCitas.Api.Models.Enums;

namespace ClinicaCitas.Api.Services;

/// <summary>Contrato de la lógica de negocio de las citas médicas (el núcleo del sistema).</summary>
public interface ICitaService
{
    /// <summary>Lista citas aplicando los filtros opcionales.</summary>
    IReadOnlyList<CitaResponseDto> Listar(CitaFiltroDto filtro);

    /// <summary>Obtiene una cita; lanza 404 si no existe.</summary>
    CitaResponseDto ObtenerPorId(int id);

    /// <summary>Citas de un paciente (ruta jerárquica); lanza 404 si el paciente no existe.</summary>
    IReadOnlyList<CitaResponseDto> ListarPorPaciente(int pacienteId, EstadoCita? estado);

    /// <summary>Agenda de un médico, opcionalmente de un día (ruta jerárquica); lanza 404 si el médico no existe.</summary>
    IReadOnlyList<CitaResponseDto> ListarPorMedico(int medicoId, DateOnly? fecha);

    /// <summary>Registra una cita validando existencia, horario de atención y disponibilidad.</summary>
    CitaResponseDto Registrar(CitaCreateRequestDto solicitud);

    /// <summary>Reprograma una cita vigente.</summary>
    CitaResponseDto Actualizar(int id, CitaUpdateRequestDto solicitud);

    /// <summary>Programada → Confirmada.</summary>
    CitaResponseDto Confirmar(int id);

    /// <summary>Programada/Confirmada → Atendida.</summary>
    CitaResponseDto Atender(int id);

    /// <summary>Programada/Confirmada → Cancelada, guardando el motivo.</summary>
    CitaResponseDto Cancelar(int id, CitaCancelRequestDto solicitud);

    /// <summary>Elimina físicamente una cita (no permitido si ya fue atendida).</summary>
    void Eliminar(int id);
}
