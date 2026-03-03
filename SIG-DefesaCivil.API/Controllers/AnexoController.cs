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
        public async Task<IActionResult> UploadArquivos([FromRoute] string ocorrenciaId, [FromForm] UploadAnexosDTO dto)
        {
            // O [FromForm] no DTO popula as propriedades automaticamente
            try
            {
                // Validação básica manual se necessário
                if (dto.Arquivos == null || !dto.Arquivos.Any())
                    return BadRequest("Nenhum arquivo enviado.");

                var anexos = await _anexoService.SalvarAnexosEmLoteAsync(dto.Arquivos, ocorrenciaId, dto.TipoEntidade);
                return Ok(anexos);
            }
            catch (ArgumentException ex) // Erros de validação (tamanho, tipo)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                // Logar ex
                return StatusCode(500, "Erro interno ao processar arquivos.");
            }
        }

        [HttpDelete("{entidadeId}/anexos")]
        public async Task<IActionResult> RemoverAnexosEmLote(
            [FromRoute] string entidadeId,
            [FromBody] RemocaoAnexosDTO dto)
        {
            try
            {
                await _anexoService.RemoverAnexosAsync(dto.TipoEntidade, entidadeId, dto.IdsAnexos);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}