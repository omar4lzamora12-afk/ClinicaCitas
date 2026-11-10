using ClinicaApi.Dtos;
using ClinicaApi.Entidades;
using ClinicaApi.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaApi.Controllers
{
    [ApiController]
    [Route("api/pacientes")]
    public class PacientesController(IPacienteService pacienteService) : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Paciente>> ObtenerTodos()
        {
            return Ok(pacienteService.ObtenerTodos());
        }

        [HttpGet("{id:int}")]
        public ActionResult<Paciente> ObtenerPorId(int id)
        {
            return Ok(pacienteService.ObtenerPorId(id));
        }

        [HttpPost]
        public ActionResult<Paciente> Post(PacienteDto dto)
        {
            var nuevoPaciente = pacienteService.Registrar(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoPaciente.Id }, nuevoPaciente);
        }
    }
}
