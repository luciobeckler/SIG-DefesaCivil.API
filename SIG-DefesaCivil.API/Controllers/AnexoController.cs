using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.Data.DTO;

namespace SIG_DefesaCivil.API.Controllers
{
    [Route("api/[controller]")] // Rota base padrão
    public class AnexoController : Controller
    {
        private readonly AnexoService _anexoService;

        public AnexoController(AnexoService anexoService)
        {
            _anexoService = anexoService;
        }

        [HttpPost("{ocorrenciaId}/anexos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAnexos([FromRoute] string ocorrenciaId, [FromForm] ListaDeAnexosDTO dto)
        {
            // O [FromForm] no DTO popula as propriedades automaticamente
            try
            {
                // Validação básica manual se necessário
                if (dto.Anexos == null || !dto.Anexos.Any())
                    return BadRequest("Nenhum anexo enviado.");

                var anexos = await _anexoService.SalvarAnexosEmLoteAsync(dto.Anexos, ocorrenciaId, dto.TipoEntidade);
                return Ok(anexos);
            }
            catch (ArgumentException ex) // Erros de validação (tamanho, tipo)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                // Logar ex
                return StatusCode(500, "Erro interno ao processar anexos.");
            }
        }

        [HttpDelete("{entidadeId}/anexos")]
        public async Task<IActionResult> RemoverAnexosEmLote(
            [FromRoute] string entidadeId,
            [FromBody] RemocaoAnexosDTO dto)
        {
            try
            {
                await _anexoService.RemoverAnexosAsync(dto.IdsAnexos);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}