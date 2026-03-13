using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Data.Enums;
using SIG_DefesaCivil.API.Mappers;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Services;
using System.Text.Json; // Necessário para desserializar

namespace SIG_DefesaCivil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class VistoriaController : Controller
    {
        private readonly OcorrenciaService _ocorrenciaService;
        // Injete o serviço de anexos que você já construiu anteriormente
        private readonly AnexoService _anexoService;
        private readonly QuadroService _quadroService;

        public VistoriaController(OcorrenciaService ocorrenciaService, AnexoService anexoService, QuadroService quadroService)
        {
            _ocorrenciaService = ocorrenciaService;
            _anexoService = anexoService;
            _quadroService = quadroService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(
        [FromForm] string dadosJson,
        [FromForm] IFormFileCollection fotos,
        [FromQuery] ETipoCadastroOcorrencia tipoCadastroOcorrencia)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dadosJson))
                    return BadRequest("Os dados da vistoria não foram enviados.");

                var dto = JsonSerializer.Deserialize<SolicitacaoVistoriaDTO>(dadosJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });


                var quadros = _quadroService.GetQuadrosId();

                var ocorrencia = await _ocorrenciaService.CriarAsync(quadros[0], dto.ToCreateOcorrenciaDTO(tipoCadastroOcorrencia), null);

                if (fotos != null && fotos.Any())
                {
                    List<AnexoUploadDTO> uploadDTOs = new List<AnexoUploadDTO>();

                    foreach (var item in fotos)
                    {
                        uploadDTOs.Add(item.ToDto());
                    }

                    await _anexoService.SalvarAnexosEmLoteAsync(uploadDTOs, ocorrencia.Id, ETiposEntidades.Ocorrencia);
                }

                return StatusCode(201, ocorrencia.Protocolo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}