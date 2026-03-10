using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.Data.Constantes;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Services;

namespace SIG_DefesaCivil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous] // Exige autenticação para todos os endpoints
    public class QuadroController : ControllerBase
    {
        private readonly QuadroService _service;

        public QuadroController(QuadroService service)
        {
            _service = service;
        }

        // GET: api/Quadro
        [HttpGet]
        [ProducesResponseType(typeof(List<QuadroDTO>), 200)]
        public async Task<IActionResult> ListarTodos()
        {
            var quadros = await _service.ListarTodosAsync();
            return Ok(quadros);
        }

        // GET: api/Quadro/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(QuadroDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObterPorId(string id)
        {
            try
            {
                var quadro = await _service.ObterPorIdAsync(id);
                return Ok(quadro);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/Quadro
        [HttpPost]
        [Authorize(Roles = Permissoes.ApenasAdmin)]
        [ProducesResponseType(typeof(QuadroDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Criar([FromBody] CriarOuEditarQuadroDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quadro = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = quadro.Id }, quadro);
        }

        // PUT: api/Quadro/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = Permissoes.ApenasAdmin)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(string id, [FromBody] CriarOuEditarQuadroDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _service.AtualizarAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/Quadro/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = Permissoes.ApenasAdmin)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Deletar(string id)
        {
            try
            {
                await _service.DeletarAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}