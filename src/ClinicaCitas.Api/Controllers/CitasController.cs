using ClinicaCitas.Api.DTOs.Citas;
using ClinicaCitas.Api.DTOs.Common;
using ClinicaCitas.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaCitas.Api.Controllers;

/// <summary>Gestión de citas médicas: registrar, consultar, reprogramar, cambiar de estado, cancelar y eliminar.</summary>
/// <remarks>
/// Resumen de códigos HTTP usados: 200 OK, 201 Created, 204 No Content, 400 datos inválidos,
/// 404 no existe, 409 conflicto (horario ocupado / estado no permitido), 422 regla de negocio incumplida, 500 error interno.
/// </remarks>
[ApiController]
[Route("api/v1/citas")]
[Produces("application/json")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public sealed class CitasController : ControllerBase
{
    private readonly ICitaService _citaService;

    public CitasController(ICitaService citaService) => _citaService = citaService;

    /// <summary>Lista las citas, con filtros opcionales combinables.</summary>
    /// <param name="filtro">Filtros por query string: fecha, estado, medicoId, pacienteId.</param>
    /// <response code="200">Citas ordenadas por fecha y hora (puede estar vacía).</response>
    /// <response code="400">Algún filtro tiene un valor inválido (por ejemplo, estado=XYZ).</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CitaResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public ActionResult<ApiResponse<IReadOnlyList<CitaResponseDto>>> Listar([FromQuery] CitaFiltroDto filtro)
    {
        var citas = _citaService.Listar(filtro);
        return Ok(ApiResponse<IReadOnlyList<CitaResponseDto>>.Correcta(citas, $"Se encontraron {citas.Count} cita(s)."));
    }

    /// <summary>Obtiene una cita por su Id.</summary>
    /// <param name="id">Identificador de la cita.</param>
    /// <response code="200">Cita encontrada.</response>
    /// <response code="404">No existe una cita con ese Id.</response>
    [HttpGet("{id}", Name = "ObtenerCitaPorId")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<CitaResponseDto>> ObtenerPorId(int id)
    {
        var cita = _citaService.ObtenerPorId(id);
        return Ok(ApiResponse<CitaResponseDto>.Correcta(cita, "Cita encontrada."));
    }

    /// <summary>Registra una nueva cita médica.</summary>
    /// <remarks>
    /// La cita nace en estado <c>Programada</c>. Ejemplo de cuerpo (la fecha debe ser futura, de lunes a sábado, dentro del horario de atención):
    ///
    ///     {
    ///       "pacienteId": 1,
    ///       "medicoId": 1,
    ///       "fechaHora": "2026-11-10T09:30:00",
    ///       "duracionMinutos": 30,
    ///       "motivo": "Dolor de cabeza persistente",
    ///       "observaciones": "Primera consulta"
    ///     }
    /// </remarks>
    /// <response code="201">Cita creada. La cabecera Location apunta a la nueva cita.</response>
    /// <response code="400">Datos inválidos (fecha pasada, motivo vacío, etc.).</response>
    /// <response code="404">El paciente o el médico no existen.</response>
    /// <response code="409">El médico o el paciente ya tienen una cita en ese horario.</response>
    /// <response code="422">Fuera del horario de atención, domingo o médico inactivo.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<CitaResponseDto>> Registrar([FromBody] CitaCreateRequestDto solicitud)
    {
        var cita = _citaService.Registrar(solicitud);
        return CreatedAtRoute("ObtenerCitaPorId", new { id = cita.Id },
            ApiResponse<CitaResponseDto>.Correcta(cita, "Cita registrada correctamente."));
    }

    /// <summary>Reprograma una cita vigente (cambia médico, horario, duración o motivo).</summary>
    /// <remarks>La cita vuelve al estado <c>Programada</c>. No se puede modificar una cita Atendida o Cancelada.</remarks>
    /// <param name="id">Identificador de la cita.</param>
    /// <param name="solicitud">Nuevos datos de la cita.</param>
    /// <response code="200">Cita reprogramada.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="404">La cita o el médico no existen.</response>
    /// <response code="409">Horario ocupado, o la cita ya no admite cambios.</response>
    /// <response code="422">Fuera del horario de atención, domingo o médico inactivo.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<CitaResponseDto>> Actualizar(int id, [FromBody] CitaUpdateRequestDto solicitud)
    {
        var cita = _citaService.Actualizar(id, solicitud);
        return Ok(ApiResponse<CitaResponseDto>.Correcta(cita, "Cita reprogramada correctamente."));
    }

    /// <summary>Confirma la asistencia del paciente (Programada → Confirmada).</summary>
    /// <param name="id">Identificador de la cita.</param>
    /// <response code="200">Cita confirmada.</response>
    /// <response code="404">No existe una cita con ese Id.</response>
    /// <response code="409">La cita no está en estado Programada.</response>
    [HttpPatch("{id}/confirmar")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<CitaResponseDto>> Confirmar(int id)
    {
        var cita = _citaService.Confirmar(id);
        return Ok(ApiResponse<CitaResponseDto>.Correcta(cita, "Cita confirmada correctamente."));
    }

    /// <summary>Marca la cita como atendida (Programada o Confirmada → Atendida).</summary>
    /// <param name="id">Identificador de la cita.</param>
    /// <response code="200">Cita marcada como atendida.</response>
    /// <response code="404">No existe una cita con ese Id.</response>
    /// <response code="409">La cita ya fue atendida o cancelada.</response>
    [HttpPatch("{id}/atender")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<CitaResponseDto>> Atender(int id)
    {
        var cita = _citaService.Atender(id);
        return Ok(ApiResponse<CitaResponseDto>.Correcta(cita, "Cita marcada como atendida."));
    }

    /// <summary>Cancela la cita conservando el registro y el motivo (Programada o Confirmada → Cancelada).</summary>
    /// <param name="id">Identificador de la cita.</param>
    /// <param name="solicitud">Motivo de la cancelación.</param>
    /// <response code="200">Cita cancelada.</response>
    /// <response code="400">Falta el motivo de la cancelación.</response>
    /// <response code="404">No existe una cita con ese Id.</response>
    /// <response code="409">La cita ya fue atendida o cancelada.</response>
    [HttpPatch("{id}/cancelar")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<CitaResponseDto>> Cancelar(int id, [FromBody] CitaCancelRequestDto solicitud)
    {
        var cita = _citaService.Cancelar(id, solicitud);
        return Ok(ApiResponse<CitaResponseDto>.Correcta(cita, "Cita cancelada correctamente."));
    }

    /// <summary>Elimina definitivamente una cita del sistema.</summary>
    /// <remarks>A diferencia de cancelar, el registro desaparece. No se permite eliminar citas Atendidas (historial clínico).</remarks>
    /// <param name="id">Identificador de la cita.</param>
    /// <response code="204">Cita eliminada.</response>
    /// <response code="404">No existe una cita con ese Id.</response>
    /// <response code="409">La cita ya fue atendida y no puede eliminarse.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public IActionResult Eliminar(int id)
    {
        _citaService.Eliminar(id);
        return NoContent();
    }
}
