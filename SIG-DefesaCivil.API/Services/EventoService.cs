using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO.Eventos;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.Services
{
    public class EventoService
    {
        private readonly DefesaCivilDbContext _context;

        public EventoService(DefesaCivilDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventoPreviewDTO>> ListarPreviewEventosAsync()
        {
            var allEventos = await _context.Eventos
                .Include(e => e.UsuarioCriador)
                .ToListAsync();
            
            var eventosPreview = allEventos.Select(x => new EventoPreviewDTO
            {
                id = x.Id,
                codigo = x.Codigo,
                titulo = x.Titulo,
                status = x.Status,
                emailResponsavel = x.UsuarioCriador.NormalizedEmail
            });
            
            return eventosPreview;
        }

        public async Task<Evento> DetalhesEventosPorId(string id, Usuario usuario)
        {
            var evento = await RecuperaEventoPorId(id);
            VerificaSeUsuarioPodeVerDetalhes(evento.UsuarioCriadorId, usuario);
            
            string acao = "Visualizou detalhes";
            AdicionaOuAtualizaHistorico(evento.Id, usuario.Id, acao);


            await _context.SaveChangesAsync();
            return evento;
        }

        private void AdicionaOuAtualizaHistorico(string eventoId, string usuarioId, string acao)
        {
            var registroUsuarioNoHistoricoEvento = _context.EventosHistoricos.FirstOrDefault(e =>
                e.Id == eventoId &&
                e.UsuarioId == usuarioId &&
                e.Acao == acao);

            if (registroUsuarioNoHistoricoEvento == null)
            {
                _context.EventosHistoricos.Add(new EventoHistorico
                {
                    EventoId = eventoId,
                    UsuarioId = usuarioId,
                    Acao = acao,
                    UltimaAlteracao = DateTime.UtcNow
                });
            }
            else
            {
                registroUsuarioNoHistoricoEvento.UltimaAlteracao = DateTime.UtcNow;
            } 
        }

        private void VerificaSeUsuarioPodeVerDetalhes(string criadorId, Usuario usuario)
        {
            var temPermissao = usuario.Cargo == "Administrador"
                  || usuario.Cargo == "Diretor"
                  || usuario.Id == criadorId;

            if (!temPermissao)
            {
                throw new UnauthorizedAccessException("Você não possui permissão para acessar os detalhes deste evento.");
            }
        }

        private async Task<Evento> RecuperaEventoPorId(string id)
        {
            var evento = await _context.Eventos
                .Include(e => e.UsuarioCriador) 
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null)
                throw new InvalidOperationException($"O evento com o ID '{id}' não foi encontrado.");

            return evento;
        }

        public async Task<Evento> CriarAsync(CreateOrEditEventoDTO dto, Usuario usuario)
        {
            await ValidarCodigoUnicoAsync(dto.Codigo);

            var evento = new Evento
            {
                Id = Guid.NewGuid().ToString(),
                Codigo = dto.Codigo,
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Endereco = dto.Endereco,
                Status = dto.Status,
                DataEHoraDoEvento = dto.DataEHoraDoEvento,
                UsuarioCriadorId = usuario.Id,
                SubEventos = new List<Evento>()
            };

            await AssociaSubEventos(evento, dto);

            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();
            return evento;
        }

        private async Task AssociaSubEventos(Evento eventoPai, CreateOrEditEventoDTO dto)
        {
            if (dto.SubEventosId != null && dto.SubEventosId.Any())
            {
                foreach (var id in dto.SubEventosId)
                {
                    var subEvento = await RecuperaEventoPorId(id);
                    eventoPai.SubEventos.Add(subEvento);

                    subEvento.EventoPai = eventoPai;
                }
            }
        }


        public async Task AtualizarAsync(string id, CreateOrEditEventoDTO dto, Usuario usuario)
        {
            
        }

        private async Task ValidarCodigoUnicoAsync(string codigo, string? eventoId = null)
        {
            var query = _context.Eventos.AsNoTracking().Where(e => e.Codigo.ToUpper() == codigo.ToUpper());

            if (eventoId != null)
            {
                query = query.Where(e => e.Id != eventoId);
            }

            if (await query.AnyAsync())
            {
                throw new InvalidOperationException($"O código '{codigo}' já está em uso por outro evento.");
            }
        }

        public async Task<bool> DeletarAsync(string id, Usuario usuario)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null) return false;

            bool podeDeletar = usuario.Cargo == "Administrador" || usuario.Cargo == "Diretor";
            if (!podeDeletar)
                throw new UnauthorizedAccessException("Você não tem permissão para excluir eventos.");

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EventoHistorico>> ListarHistoricoAsync(string eventoId, Usuario usuario)
        {
            if (usuario.Cargo != "Administrador" && usuario.Cargo != "Diretor")
                throw new UnauthorizedAccessException("Acesso restrito aos administradores e diretores.");

            return await _context.EventosHistoricos
                .Include(h => h.Usuario)
                .Where(h => h.EventoId == eventoId)
                .OrderByDescending(h => h.UltimaAlteracao)
                .ToListAsync();
        }
    }
}
