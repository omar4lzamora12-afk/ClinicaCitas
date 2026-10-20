using ClinicaCitas.Api.DTOs.Citas;
using ClinicaCitas.Api.DTOs.Common;
using ClinicaCitas.Api.DTOs.Pacientes;
using ClinicaCitas.Api.Models.Enums;
using ClinicaCitas.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaCitas.Api.Controllers;

/// <summary>Administración de pacientes y consulta de sus citas.</summary>
/// <remarks>
/// Capa de presentación: solo recibe la petición HTTP, delega en el servicio y arma la respuesta.
/// No contiene lógica de negocio ni bloques try/catch (los errores los maneja <c>GlobalExceptionHandler</c>).
/// </remarks>
[ApiController]
[Route("api/v1/pacientes")]
[Produces("application/json")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public sealed class PacientesController : ControllerBase
{
    private readonly IPacienteService _pacienteService;
    private readonly ICitaService _citaService;

    public PacientesController(IPacienteService pacienteService, ICitaService citaService)
    {
        _pacienteService = pacienteService;
        _citaService = citaService;
    }

    /// <summary>Lista todos los pacientes registrados.</summary>
    /// <response code="200">Lista de pacientes (puede estar vacía).</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PacienteResponseDto>>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<IReadOnlyList<PacienteResponseDto>>> Listar()
    {
        var pacientes = _pacienteService.ObtenerTodos();
        return Ok(ApiResponse<IReadOnlyList<PacienteResponseDto>>.Correcta(pacientes, $"Se encontraron {pacientes.Count} paciente(s)."));
    }

    /// <summary>Obtiene un paciente por su Id.</summary>
    /// <param name="id">Identificador del paciente.</param>
    /// <response code="200">Paciente encontrado.</response>
    /// <response code="400">El Id no es un número válido.</response>
    /// <response code="404">No existe un paciente con ese Id.</response>
    [HttpGet("{id}", Name = "ObtenerPacientePorId")]
    [ProducesResponseType(typeof(ApiResponse<PacienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<PacienteResponseDto>> ObtenerPorId(int id)
    {
        var paciente = _pacienteService.ObtenerPorId(id);
        return Ok(ApiResponse<PacienteResponseDto>.Correcta(paciente, "Paciente encontrado."));
    }

    /// <summary>Registra un nuevo paciente.</summary>
    /// <remarks>
    /// Ejemplo de cuerpo:
    ///
    ///     {
    ///       "nombres": "Lucía",
    ///       "apellidos": "Fernández Rojas",
    ///       "dni": "45871236",
    ///       "telefono": "987654321",
    ///       "email": "lucia@correo.com",
    ///       "fechaNacimiento": "1990-05-14",
    ///       "direccion": "Av. Javier Prado 1200, San Isidro"
    ///     }
    /// </remarks>
    /// <response code="201">Paciente creado. La cabecera Location apunta al nuevo recurso.</response>
    /// <response code="400">Datos inválidos (ver el detalle por campo en "errores").</response>
    /// <response code="409">Ya existe un paciente con ese DNI.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PacienteResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<PacienteResponseDto>> Registrar([FromBody] PacienteRequestDto solicitud)
    {
        var paciente = _pacienteService.Registrar(solicitud);
        return CreatedAtRoute("ObtenerPacientePorId", new { id = paciente.Id },
            ApiResponse<PacienteResponseDto>.Correcta(paciente, "Paciente registrado correctamente."));
    }

    /// <summary>Reemplaza los datos de un paciente existente.</summary>
    /// <param name="id">Identificador del paciente.</param>
    /// <param name="solicitud">Nuevos datos (se envían todos los campos).</param>
    /// <response code="200">Paciente actualizado.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="404">No existe un paciente con ese Id.</response>
    /// <response code="409">El DNI pertenece a otro paciente.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PacienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<PacienteResponseDto>> Actualizar(int id, [FromBody] PacienteRequestDto solicitud)
    {
        var paciente = _pacienteService.Actualizar(id, solicitud);
        return Ok(ApiResponse<PacienteResponseDto>.Correcta(paciente, "Paciente actualizado correctamente."));
    }

    /// <summary>Elimina un paciente que no tenga citas registradas.</summary>
    /// <param name="id">Identificador del paciente.</param>
    /// <response code="204">Paciente eliminado (sin contenido en la respuesta).</response>
    /// <response code="404">No existe un paciente con ese Id.</response>
    /// <response code="409">El paciente tiene citas registradas.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public IActionResult Eliminar(int id)
    {
        _pacienteService.Eliminar(id);
        return NoContent();
    }

    /// <summary>Lista las citas de un paciente (ruta jerárquica: el paciente "contiene" sus citas).</summary>
    /// <param name="pacienteId">Identificador del paciente.</param>
    /// <param name="estado">Filtro opcional: Programada, Confirmada, Atendida o Cancelada.</param>
    /// <response code="200">Citas del paciente (puede estar vacía).</response>
    /// <response code="404">No existe un paciente con ese Id.</response>
    [HttpGet("{pacienteId}/citas")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CitaResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<IReadOnlyList<CitaResponseDto>>> ListarCitas(int pacienteId, [FromQuery] EstadoCita? estado)
    {
        var citas = _citaService.ListarPorPaciente(pacienteId, estado);
        return Ok(ApiResponse<IReadOnlyList<CitaResponseDto>>.Correcta(citas, $"El paciente tiene {citas.Count} cita(s)."));
    }
}
