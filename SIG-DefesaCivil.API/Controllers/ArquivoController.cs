using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.Services;

namespace SIG_DefesaCivil.API.Controllers
{
    public class ArquivoController : Controller
    {
        private readonly AnexoService _anexoService;
        private readonly UsuarioService _usuarioService;
        private readonly OcorrenciaService _ocorrenciaService;
        public ArquivoController(AnexoService anexoService, UsuarioService usuarioService, OcorrenciaService ocorrenciaService)
        {
            _anexoService = anexoService;
            _usuarioService = usuarioService;
            _ocorrenciaService = ocorrenciaService;
        }

        [HttpDelete("{entidadeId}/anexos")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> RemoverAnexosEmLote(
        [FromBody] string entidadeTipo,
        [FromRoute] string entidadeId,
        List<string> idsAnexosParaRemover)
        {
            try
            {
                if (idsAnexosParaRemover == null || !idsAnexosParaRemover.Any())
                    return BadRequest("Lista de IDs para remoção vazia.");

                await _anexoService.RemoverAnexosAsync(entidadeTipo, entidadeId, idsAnexosParaRemover);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id}/anexos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadArquivos(
        [FromRoute] string ocorrenciaId,
        [FromBody] string entidade,
        [FromForm] List<IFormFile> arquivos)
        {
            try
            {
                var ocorrencia = await _ocorrenciaService.GetOcorrenciaPreviewById(ocorrenciaId);
                var anexosDto = await _anexoService.SalvarAnexoAsync(arquivos, ocorrenciaId, entidade);

                return Ok(anexosDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
