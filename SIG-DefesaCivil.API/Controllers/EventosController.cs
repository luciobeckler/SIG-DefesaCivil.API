using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIG_DefesaCivil.API.DTO.Eventos;
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

        public EventoController(EventoService service, UserManager<Usuario> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        // --- MÉTODOS PRIVADOS AUXILIARES ---

        private async Task<Usuario> GetUsuarioAtual()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Usuário não encontrado no token.");
            }
            return await _userManager.FindByIdAsync(userId);
        }

        // NOVO MÉTODO DE MAPEAMENTO PARA EVITAR REPETIÇÃO DE CÓDIGO (DRY PRINCIPLE)
        private EventoDetalhesDTO MapearParaDetalhesDTO(Evento evento)
        {
            if (evento == null) return null;

            return new EventoDetalhesDTO
            {
                Id = evento.Id,
                Codigo = evento.Codigo,
                Titulo = evento.Titulo,
                Descricao = evento.Descricao,
                Endereco = evento.Endereco,
                Status = evento.Status,
                DataEHoraDoEvento = evento.DataEHoraDoEvento,
                EventoPaiId = evento.EventoPaiId,
                UsuarioCriador = evento.UsuarioCriador != null ? new EventoDetalhesUsuarioDTO
                {
                    Id = evento.UsuarioCriador.Id,
                    Nome = evento.UsuarioCriador.UserName,
                    Email = evento.UsuarioCriador.Email
                } : null,
                SubEventos = evento.SubEventos?.Select(sub => new EventoDetalhesSubEventoDTO
                {
                    Id = sub.Id,
                    Codigo = sub.Codigo,
                    Titulo = sub.Titulo
                }).ToList() ?? new List<EventoDetalhesSubEventoDTO>()
            };
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
                var evento = await _service.DetalhesEventosPorId(id, usuario);

                var eventoDto = MapearParaDetalhesDTO(evento);

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

                var eventoDto = MapearParaDetalhesDTO(eventoEntity);

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
            return Ok(historico);
        }
    }
}