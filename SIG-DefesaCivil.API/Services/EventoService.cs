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

        //!todo corrigir erro do get, ver gemini
        public async Task<Evento> DetalhesEventosPorId(string id, Usuario usuario)
        {
            var evento = await RecuperaEventoCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(evento.UsuarioCriadorId, usuario);
            
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

        private async Task<Evento> RecuperaEventoCompletoPorId(string id)
        {
            await VerificaSeEventoExiste(id);
            
            var evento = await _context.Eventos
                .Include(e => e.UsuarioCriador)
                .Include(e => e.SubEventos)
                .FirstOrDefaultAsync(e => e.Id == id);

            return evento;
        }

        private async Task VerificaSeEventoExiste(string id)
        {
            var isEventoExiste = await _context.Eventos.AnyAsync(e => e.Id == id);
            if (!isEventoExiste)
                throw new InvalidOperationException($"O evento com o ID '{id}' não foi encontrado.");
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
                    var subEvento = await RecuperaEventoCompletoPorId(id);
                    eventoPai.SubEventos.Add(subEvento);

                    subEvento.EventoPai = eventoPai;
                }
            }
        }


        public async Task AtualizarAsync(string id, CreateOrEditEventoDTO dto, Usuario usuario)
        {
            await ValidarCodigoUnicoAsync(dto.Codigo, id);

            var evento = await RecuperaEventoCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(evento.UsuarioCriadorId, usuario); 

            evento.Codigo = dto.Codigo;
            evento.Titulo = dto.Titulo;
            evento.Descricao = dto.Descricao;
            evento.Endereco = dto.Endereco;
            evento.Status = dto.Status;
            evento.DataEHoraDoEvento = dto.DataEHoraDoEvento;

            //!TODO criar método para reutilizar esta verificação
            if (string.IsNullOrWhiteSpace(dto.EventoPaiId))
            {
                evento.EventoPai = null;
            }
            else
            {
                await VerificaSeEventoExiste(dto.EventoPaiId);
                evento.EventoPaiId = dto.EventoPaiId;
            }
            await AtualizaRelacionamentoSubEventosAsync(evento, dto);

            var acao = "Editou evento";
            AdicionaOuAtualizaHistorico(evento.Id, usuario.Id, acao);

            await _context.SaveChangesAsync();
        }

        private async Task AtualizaRelacionamentoSubEventosAsync(Evento eventoParaAtualizar, CreateOrEditEventoDTO dto)
        {
            var novosIds = dto.SubEventosId?.ToHashSet() ?? new HashSet<string>();

            if (novosIds.Contains(eventoParaAtualizar.Id))
            {
                throw new InvalidOperationException("Um evento não pode ser definido como seu próprio sub-evento.");
            }
            var idsAtuais = eventoParaAtualizar.SubEventos.Select(s => s.Id).ToHashSet();

            var subEventosParaRemover = eventoParaAtualizar.SubEventos
                .Where(s => !novosIds.Contains(s.Id))
                .ToList();

            foreach (var subEvento in subEventosParaRemover)
            {
                eventoParaAtualizar.SubEventos.Remove(subEvento);
            }

            var idsParaAdicionar = novosIds.Where(id => !idsAtuais.Contains(id)).ToList();

            if (idsParaAdicionar.Any())
            {
                var subEventosParaAdicionar = await _context.Eventos
                    .Where(e => idsParaAdicionar.Contains(e.Id))
                    .ToListAsync();

                if (subEventosParaAdicionar.Count != idsParaAdicionar.Count)
                {
                    var idsEncontrados = subEventosParaAdicionar.Select(e => e.Id).ToList();
                    var idsNaoEncontrados = idsParaAdicionar.Except(idsEncontrados);

                    throw new InvalidOperationException($"Os seguintes IDs de sub-eventos não foram encontrados: {string.Join(", ", idsNaoEncontrados)}");
                }

                foreach (var subEvento in subEventosParaAdicionar)
                {
                    eventoParaAtualizar.SubEventos.Add(subEvento);
                }
            }
        }

        private void VerificaSeUsuarioPossuiPermissao(string criadorId, Usuario usuario)
        {
            var temPermissao = usuario.Cargo == "ADMINISTRADOR"
                    || usuario.Cargo == "DIRETOR"
                    || usuario.Id == criadorId;

            if (!temPermissao)
            {
                throw new UnauthorizedAccessException("Você não possui permissão para editar este evento.");
            }
        }

        private async Task ValidarCodigoUnicoAsync(string codigo, string? eventoIgnoradoId = null)
        {
            var query = _context.Eventos.AsNoTracking().Where(e => e.Codigo.ToUpper() == codigo.ToUpper());

            if (eventoIgnoradoId != null)
            {
                query = query.Where(e => e.Id != eventoIgnoradoId);
            }

            if (await query.AnyAsync())
            {
                throw new InvalidOperationException($"O código '{codigo}' já está em uso por outro evento.");
            }
        }

        public async Task DeletarAsync(string id, Usuario usuario)
        {
            var evento = await _context.Eventos
                .Include(e => e.SubEventos)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null)
            {
                throw new InvalidOperationException($"O evento com o ID '{id}' não foi encontrado.");
            }

            bool podeDeletar = usuario.Cargo.ToUpper() == "ADMINISTRADOR" || usuario.Cargo.ToUpper() == "DIRETOR";
            if (!podeDeletar)
                throw new UnauthorizedAccessException("Você não tem permissão para excluir eventos.");

            if (evento.SubEventos != null && evento.SubEventos.Any())
            {
                throw new InvalidOperationException("Não é possível excluir este evento pois ele possui sub-eventos associados. Remova ou reatribua os sub-eventos primeiro.");
            }

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EventoHistorico>> ListarHistoricoAsync(string eventoId, Usuario usuario)
        {
            if (usuario.Cargo != "ADMINISTRADOR" && usuario.Cargo != "DIRETOR")
                throw new UnauthorizedAccessException("Acesso restrito aos administradores e diretores.");

            return await _context.EventosHistoricos
                .Include(h => h.Usuario)
                .Where(h => h.EventoId == eventoId)
                .OrderByDescending(h => h.UltimaAlteracao)
                .ToListAsync();
        }
    }
}
