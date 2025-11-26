using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.DTO.Etapas;
using SIG_DefesaCivil.API.Services;

namespace SIG_DefesaCivil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class EtapaController : ControllerBase
    {
        private readonly EtapaService _service;

        public EtapaController(EtapaService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriaOuAtualizaEtapaDTO dto)
        {
            try
            {
                var etapa = await _service.CriarAsync(dto);
                return Ok(etapa);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(string id, [FromBody] CriaOuAtualizaEtapaDTO dto)
        {
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

        [HttpPut("reordenar/{quadroId}")]
        public async Task<IActionResult> Reordenar(string quadroId, [FromBody] ReordenarEtapaDTO dto)
        {
            // Este método permite que qualquer usuário autenticado reordene (ou restrinja se preferir)
            await _service.ReordenarEtapasAsync(quadroId, dto.IdsDasEtapasNaOrdem);
            return NoContent();
        }

        [HttpDelete("{id}")]
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
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); // 409 se tiver eventos
            }
        }
    }
}