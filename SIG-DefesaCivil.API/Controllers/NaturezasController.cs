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
        public async Task<ActionResult<IEnumerable<NaturezaDto>>> GetNaturezas()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NaturezaDto>> GetNatureza(string id)
        {
            var natureza = await _service.GetByIdAsync(id);
            if (natureza == null) return NotFound();
            return natureza;
        }

        [HttpPost]
        public async Task<ActionResult<Natureza>> PostNatureza(CreateNaturezaDto dto)
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
        public async Task<IActionResult> PutNatureza(string id, CreateNaturezaDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (!updated) return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNatureza(string id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}/irmas")]
        public async Task<ActionResult<IEnumerable<NaturezaDto>>> GetIrmas(string id)
        {
            return await _service.GetIrmasAsync(id);
        }
    }
}
