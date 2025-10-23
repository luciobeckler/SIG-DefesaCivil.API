using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.DTOs;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Services;

namespace SIG_DefesaCivil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class NaturezasController : ControllerBase
    {
        private readonly NaturezaService _service;

        public NaturezasController(NaturezaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NaturezaDTO>>> GetNaturezas()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NaturezaDTO>> GetNatureza(string codigo)
        {
            try
            {
                var natureza = await _service.GetByCodigoAsync(codigo);
                return Ok(natureza);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Natureza>> PostNatureza(CreateNaturezaDTO dto)
        {
            try
            {
                var natureza = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetNatureza), new { id = natureza.Id }, natureza);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNatureza(string id, CreateNaturezaDTO dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteNatureza(string codigo)
        {
            try
            {
                var deleted = await _service.DeleteAsync(codigo);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}/irmas")]
        public async Task<ActionResult<IEnumerable<NaturezaDTO>>> GetIrmas(string id)
        {
            return await _service.GetIrmasAsync(id);
        }
    }
}
