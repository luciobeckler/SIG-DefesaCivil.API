using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [ProducesResponseType(typeof(EventoDetalhesDTO), 201)]
        [ProducesResponseType(400)] 
        [ProducesResponseType(409)]
        public async Task<IActionResult> Create([FromBody] CreateOrEditEventoDTO dto)
        {
            try
            {
                var usuario = await GetUsuarioAtual();
                var eventoEntity = await _service.CriarAsync(dto, usuario);
                var eventoDto = _mapper.Map<EventoDetalhesDTO>(eventoEntity);

                return CreatedAtAction(nameof(GetDetalhesById), new { id = eventoDto.Id }, eventoDto);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Update(string id, [FromBody] CreateOrEditEventoDTO dto)
        {
            try
            {
                var usuario = await GetUsuarioAtual();
                await _service.AtualizarAsync(id, dto, usuario);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("não foi encontrado"))
                    return NotFound(new { message = ex.Message });
                else
                    return Conflict(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
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