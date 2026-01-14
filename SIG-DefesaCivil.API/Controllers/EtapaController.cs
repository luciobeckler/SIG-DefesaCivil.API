using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.Constants;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Services;

namespace SIG_DefesaCivil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Permissoes.ApenasAdmin)]
    public class EtapaController : ControllerBase
    {
        private readonly EtapaService _service;
        private readonly UserManager<Usuario> _userManager;

        public EtapaController(EtapaService service, UserManager<Usuario> userManager)
        {
            _service = service;
        }

        // POST: api/Etapa
        [HttpPost]
        [ProducesResponseType(typeof(EtapaDTO), 201)] // Retorna 200 no seu service, mas CreatedAt seria melhor se possível
        [ProducesResponseType(400)]
        [ProducesResponseType(404)] // Se o quadro não existir
        public async Task<IActionResult> Criar([FromBody] CriaOuAtualizaEtapaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var etapa = await _service.CriarAsync(dto);
                // Como EtapaService.CriarAsync retorna EtapaDTO, podemos retornar Ok(etapa) ou Created
                // Idealmente seria CreatedAtAction, mas precisaria de um endpoint GetById na EtapaController
                return Ok(etapa);
            }
            catch (KeyNotFoundException ex) // Quadro não encontrado
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex) // Erro de validação (ex: FormularioId inválido)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Etapa/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(string id, [FromBody] CriaOuAtualizaEtapaDTO dto)
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
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Etapa/reordenar/{quadroId}
        [HttpPut("reordenar/{quadroId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Reordenar(string quadroId, [FromBody] ReordenarEtapaDTO dto)
        {
            // Permite que usuários autenticados reordenem (necessário para drag-and-drop no Kanban)
            // Se quiser restringir, adicione [Authorize(Roles = ...)]
            await _service.ReordenarEtapasAsync(quadroId, dto.IdsDasEtapasNaOrdem);
            return NoContent();
        }

        // DELETE: api/Etapa/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)] // Conflict se tiver ocorrencias
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
            catch (InvalidOperationException ex) // Se a etapa tiver ocorrencias
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}