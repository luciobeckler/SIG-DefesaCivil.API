using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.DTO.Eventos;
using SIG_DefesaCivil.API.Enums;
using SIG_DefesaCivil.API.Models.Eventos;
using SIG_DefesaCivil.API.Services;

namespace SIG_DefesaCivil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventoController : ControllerBase
    {
        private readonly EventoService _eventoService;
        private readonly UsuarioService _usuarioService;
        private readonly IMapper _mapper;

        public EventoController(EventoService eventoService, UsuarioService usuarioService, IMapper mapper)
        {
            _eventoService = eventoService;
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        [HttpGet("{id}/detalhes")]
        [ProducesResponseType(typeof(EventoDetalhesDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetDetalhesById(string id)
        {
            try
            {
                var usuario = await _usuarioService.GetUsuarioAtual(User);
                var eventoDto = await _eventoService.DetalhesEventosPorId(id, usuario);
                eventoDto.Anexos = await _eventoService.GetAnexosDTOByEventoIdAsync(id);

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
        [Consumes("application/json")] 
        [ProducesResponseType(typeof(EventoDetalhesDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(
        [FromBody] CreateOrEditEventoDTO dto,
        [FromQuery] string etapaId)
        {
            try
            {
                var usuario = await _usuarioService.GetUsuarioAtual(User);
                var eventoEntity = await _eventoService.CriarAsync(usuario, etapaId, dto);
                var eventoDto = _mapper.Map<EventoDetalhesDTO>(eventoEntity);

                return CreatedAtAction(nameof(GetDetalhesById), new { id = eventoEntity.Id }, eventoDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Consumes("application/json")] 
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Update(string id,
        [FromBody] CreateOrEditEventoDTO dto)
        {
            try
            {
                var usuario = await _usuarioService.GetUsuarioAtual(User);
                await _eventoService.AtualizarAsync(id, dto, usuario);
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
            var anexos = await _eventoService.GetAnexosDTOByEventoIdAsync(id);
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
                var usuario = await _usuarioService.GetUsuarioAtual(User);
                await _eventoService.DeletarAsync(id, usuario);
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
            var usuario = await _usuarioService.GetUsuarioAtual(User);
            var historico = await _eventoService.ListarHistoricoAsync(id, usuario);
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