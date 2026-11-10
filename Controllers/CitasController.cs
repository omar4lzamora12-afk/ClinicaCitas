using ClinicaApi.Dtos;
using ClinicaApi.Entidades;
using ClinicaApi.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaApi.Controllers
{
    [ApiController] // Decorado para realizar validaciones que envía el usuario
    [Route("api/citas")] // Ruta para llamar a las acciones de un controlador
    public class CitasController(ICitaService citaService) : ControllerBase // DIP: recibe la interfaz desde afuera
    {
        [HttpGet]
        public ActionResult<List<Cita>> ObtenerTodas()
        {
            return Ok(citaService.ObtenerTodas());
        }

        [HttpGet("{id:int}")]
        public ActionResult<Cita> ObtenerPorId(int id) // Método para devolver datos de la Cita por ID
        {
            return Ok(citaService.ObtenerPorId(id));
        }

        [HttpPost]
        public ActionResult<Cita> Post(CitaDto dto) // Registra una nueva cita
        {
            var nuevaCita = citaService.Registrar(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaCita.Id }, nuevaCita);
        }

        [HttpPut("{id:int}")]
        public ActionResult<Cita> Put(int id, CitaDto dto) // Actualiza los datos de una cita
        {
            return Ok(citaService.Actualizar(id, dto));
        }

        [HttpPatch("{id:int}/cancelar")]
        public ActionResult<Cita> Cancelar(int id) // Cambia el estado de la cita a Cancelada
        {
            return Ok(citaService.Cancelar(id));
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id) // Elimina la cita del listado
        {
            citaService.Eliminar(id);
            return NoContent();
        }
    }
}
