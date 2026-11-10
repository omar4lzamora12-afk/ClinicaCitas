using ClinicaApi.Dtos;
using ClinicaApi.Entidades;
using ClinicaApi.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaApi.Controllers
{
    [ApiController]
    [Route("api/medicos")]
    public class MedicosController(IMedicoService medicoService) : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Medico>> ObtenerTodos()
        {
            return Ok(medicoService.ObtenerTodos());
        }

        [HttpGet("{id:int}")]
        public ActionResult<Medico> ObtenerPorId(int id)
        {
            return Ok(medicoService.ObtenerPorId(id));
        }

        [HttpPost]
        public ActionResult<Medico> Post(MedicoDto dto)
        {
            var nuevoMedico = medicoService.Registrar(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoMedico.Id }, nuevoMedico);
        }
    }
}
