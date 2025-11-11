using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.DTO.Eventos;
using SIG_DefesaCivil.API.DTO.Eventos.SIG_DefesaCivil.API.DTO.Eventos;
using SIG_DefesaCivil.API.Enums;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;
using SIG_DefesaCivil.API.Services;
using System.Security.Claims;

namespace SIG_DefesaCivil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventoController : ControllerBase
    {
        private readonly EventoService _service;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;

        public EventoController(EventoService service, UserManager<Usuario> userManager, IMapper mapper)
        {
            _service = service;
            _userManager = userManager;
            _mapper = mapper;
        }
        private async Task<Usuario> GetUsuarioAtual()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Usuário não encontrado no token.");
            }
            return await _userManager.FindByIdAsync(userId);
        }

        [HttpGet("getAllPreview")]
        [ProducesResponseType(typeof(IEnumerable<EventoPreviewDTO>), 200)]
        public async Task<IActionResult> GetAllPreview()
        {
            var eventos = await _service.ListarPreviewEventosAsync();
            return Ok(eventos);
        }

        [HttpGet("{id}/detalhes")]
        [ProducesResponseType(typeof(EventoDetalhesDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDetalhesById(string id)
        {
            try
            {
                var usuario = await GetUsuarioAtual();
                var eventoDto = await _service.DetalhesEventosPorId(id, usuario);
                eventoDto.Anexos = await _service.GetAnexosDTOByEventoIdAsync(id);

                return Ok(eventoDto);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")] // Especifica o tipo de conteúdo esperado
        [ProducesResponseType(typeof(EventoDetalhesDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Create(
        [FromForm] CreateOrEditEventoDTO dto,         // 1. [FromForm] para o DTO
        [FromForm(Name = "anexos")] List<IFormFile>? anexos // 2. [FromForm] para os arquivos
    )
        {
            try
            {
                var usuario = await GetUsuarioAtual();

                // 3. Passe o DTO e os arquivos para o serviço
                var eventoEntity = await _service.CriarAsync(dto, anexos, usuario);

                // 4. Mapeie para o DTO de *detalhes* para a resposta
                var eventoDto = _mapper.Map<EventoDetalhesDTO>(eventoEntity);

                // 5. Adicione os anexos recém-criados ao DTO de resposta
                //    (Opcional, mas bom para o front-end ter os IDs e URLs imediatamente)
                eventoDto.Anexos = await _service.GetAnexosDTOByEventoIdAsync(eventoEntity.Id);

                return CreatedAtAction(nameof(GetDetalhesById), new { id = eventoEntity.Id }, eventoDto);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); } // Erros de validação (ex: arquivo)
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); } // Regras de negócio
            catch (Exception ex)
            {
                // Log ex
                return StatusCode(500, "Ocorreu um erro interno ao processar a solicitação.");
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Update(
            string id,
            [FromForm] CreateOrEditEventoDTO dto,
            [FromForm(Name = "anexos")] List<IFormFile>? anexosNovos,
            [FromForm] List<string>? anexosParaRemoverIds
        )
        {
            try
            {
                var usuario = await GetUsuarioAtual();
                await _service.AtualizarAsync(id, dto, anexosNovos, anexosParaRemoverIds, usuario);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("não foi encontrado"))
                    return NotFound(new { message = ex.Message });
                else
                    return Conflict(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro interno ao processar a solicitação.");
            }
        }

        [HttpGet("{id}/anexos")]
        [ProducesResponseType(typeof(IEnumerable<AnexoDTO>), 200)]
        public async Task<IActionResult> GetAnexos(string id)
        {
            var anexos = await _service.GetAnexosDTOByEventoIdAsync(id);
            return Ok(anexos);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var usuario = await GetUsuarioAtual();
                await _service.DeletarAsync(id, usuario);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/historico")]
        public async Task<IActionResult> GetHistorico(string id)
        {
            var usuario = await GetUsuarioAtual();
            var historico = await _service.ListarHistoricoAsync(id, usuario);
            var historicoDTO = historico
                .Select(h => new HistoricoEventoDTO
                {
                    Id = h.Id,
                    EventoId = h.EventoId,
                    UsuarioId = h.UsuarioId,
                    Acao = h.Acao,
                    UltimaAlteracao = h.UltimaAlteracao
                });
            return Ok(historicoDTO);
        }

        [HttpGet("status")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public IActionResult GetStatusOptions()
        {
            var statusOptions = Enum.GetNames(typeof(EStatusEvento))
                                    .Select(statusName => new
                                    {
                                        value = statusName,
                                        displayName = statusName
                                    })
                                    .ToList();

            return Ok(statusOptions);
        }
    }
}