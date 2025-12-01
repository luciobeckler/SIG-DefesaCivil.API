using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.Services;

namespace SIG_DefesaCivil.API.Controllers
{
    public class ArquivoController : Controller
    {
        private readonly AnexoService _anexoService;
        private readonly UsuarioService _usuarioService;
        private readonly EventoService _eventoService;
        public ArquivoController(AnexoService anexoService, UsuarioService usuarioService, EventoService eventoService)
        {
            _anexoService = anexoService;
            _usuarioService = usuarioService;
            _eventoService = eventoService;
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

                await _anexoService.RemoverAnexosAsync(entidadeTipo ,entidadeId, idsAnexosParaRemover);
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
        [FromRoute] string eventoId,
        [FromBody] string entidade,
        [FromForm] List<IFormFile> arquivos)
        {
            try
            {
                var evento = await _eventoService.GetEventoPreviewById(eventoId);
                var anexosDto = await _anexoService.SalvarAnexoAsync(arquivos, eventoId, entidade);

                return Ok(anexosDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
