using ClinicaCitas.Api.DTOs.Citas;
using ClinicaCitas.Api.DTOs.Common;
using ClinicaCitas.Api.DTOs.Medicos;
using ClinicaCitas.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaCitas.Api.Controllers;

/// <summary>Administración de médicos y consulta de su agenda.</summary>
[ApiController]
[Route("api/v1/medicos")]
[Produces("application/json")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public sealed class MedicosController : ControllerBase
{
    private readonly IMedicoService _medicoService;
    private readonly ICitaService _citaService;

    public MedicosController(IMedicoService medicoService, ICitaService citaService)
    {
        _medicoService = medicoService;
        _citaService = citaService;
    }

    /// <summary>Lista los médicos registrados.</summary>
    /// <param name="especialidad">Filtro opcional por especialidad (coincidencia parcial, sin distinguir mayúsculas).</param>
    /// <response code="200">Lista de médicos (puede estar vacía).</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MedicoResponseDto>>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<IReadOnlyList<MedicoResponseDto>>> Listar([FromQuery] string? especialidad)
    {
        var medicos = _medicoService.ObtenerTodos(especialidad);
        return Ok(ApiResponse<IReadOnlyList<MedicoResponseDto>>.Correcta(medicos, $"Se encontraron {medicos.Count} médico(s)."));
    }

    /// <summary>Obtiene un médico por su Id.</summary>
    /// <param name="id">Identificador del médico.</param>
    /// <response code="200">Médico encontrado.</response>
    /// <response code="404">No existe un médico con ese Id.</response>
    [HttpGet("{id}", Name = "ObtenerMedicoPorId")]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<MedicoResponseDto>> ObtenerPorId(int id)
    {
        var medico = _medicoService.ObtenerPorId(id);
        return Ok(ApiResponse<MedicoResponseDto>.Correcta(medico, "Médico encontrado."));
    }

    /// <summary>Registra un nuevo médico.</summary>
    /// <remarks>
    /// Ejemplo de cuerpo:
    ///
    ///     {
    ///       "nombres": "Patricia",
    ///       "apellidos": "Villanueva León",
    ///       "dni": "09234567",
    ///       "telefono": "944223344",
    ///       "email": "patricia@clinica.pe",
    ///       "numeroColegiatura": "051234",
    ///       "especialidad": "Pediatría",
    ///       "activo": true
    ///     }
    /// </remarks>
    /// <response code="201">Médico creado.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="409">El DNI o la colegiatura ya están registrados.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<MedicoResponseDto>> Registrar([FromBody] MedicoRequestDto solicitud)
    {
        var medico = _medicoService.Registrar(solicitud);
        return CreatedAtRoute("ObtenerMedicoPorId", new { id = medico.Id },
            ApiResponse<MedicoResponseDto>.Correcta(medico, "Médico registrado correctamente."));
    }

    /// <summary>Reemplaza los datos de un médico (también sirve para activarlo o desactivarlo).</summary>
    /// <param name="id">Identificador del médico.</param>
    /// <param name="solicitud">Nuevos datos (se envían todos los campos).</param>
    /// <response code="200">Médico actualizado.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="404">No existe un médico con ese Id.</response>
    /// <response code="409">El DNI o la colegiatura pertenecen a otro médico.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<MedicoResponseDto>> Actualizar(int id, [FromBody] MedicoRequestDto solicitud)
    {
        var medico = _medicoService.Actualizar(id, solicitud);
        return Ok(ApiResponse<MedicoResponseDto>.Correcta(medico, "Médico actualizado correctamente."));
    }

    /// <summary>Elimina un médico que no tenga citas registradas.</summary>
    /// <param name="id">Identificador del médico.</param>
    /// <response code="204">Médico eliminado.</response>
    /// <response code="404">No existe un médico con ese Id.</response>
    /// <response code="409">El médico tiene citas registradas.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public IActionResult Eliminar(int id)
    {
        _medicoService.Eliminar(id);
        return NoContent();
    }

    /// <summary>Agenda de un médico (ruta jerárquica), opcionalmente de un solo día.</summary>
    /// <param name="medicoId">Identificador del médico.</param>
    /// <param name="fecha">Filtro opcional por día, formato AAAA-MM-DD.</param>
    /// <response code="200">Citas del médico ordenadas por fecha y hora.</response>
    /// <response code="404">No existe un médico con ese Id.</response>
    [HttpGet("{medicoId}/citas")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CitaResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<IReadOnlyList<CitaResponseDto>>> ListarCitas(int medicoId, [FromQuery] DateOnly? fecha)
    {
        var citas = _citaService.ListarPorMedico(medicoId, fecha);
        return Ok(ApiResponse<IReadOnlyList<CitaResponseDto>>.Correcta(citas, $"El médico tiene {citas.Count} cita(s)."));
    }
}
