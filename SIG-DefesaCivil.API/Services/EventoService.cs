using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO.Eventos.SIG_DefesaCivil.API.DTO.Eventos;
using SIG_DefesaCivil.API.DTOs;
using SIG_DefesaCivil.API.Enums;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.Services
{
    public class EventoService
    {
        private readonly DefesaCivilDbContext _context;
        private readonly IMapper _mapper;
        private readonly NaturezaService _naturezaService;

        public EventoService(DefesaCivilDbContext context, IMapper mapper, NaturezaService naturezaService)
        {
            _context = context;
            _mapper = mapper;
            _naturezaService = naturezaService;
        }
        public async Task<IEnumerable<EventoPreviewDTO>> ListarPreviewEventosAsync()
        {
            var allEventos = await _context.Eventos
                .Where(e => e.isVisible == true)
                .Include(e => e.UsuarioCriador)
                .Include(e => e.Naturezas) 
                .AsNoTracking()
                .ToListAsync();


            return _mapper.Map<IEnumerable<EventoPreviewDTO>>(allEventos);
        }

        public async Task<EventoDetalhesDTO> DetalhesEventosPorId(string id, Usuario usuario)
        {
            var evento = await RecuperaEventoCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(evento.UsuarioCriadorId, usuario);

            string acao = "Visualizou detalhes";
            AdicionaOuAtualizaHistorico(evento.Id, usuario.Id, acao);

            var eventoDto = _mapper.Map<EventoDetalhesDTO>(evento);

            await _context.SaveChangesAsync();
            return eventoDto;
        }

        public async Task<Evento> CriarAsync(CreateOrEditEventoDTO dto, Usuario usuario)
        {
            await ValidarCodigoUnicoAsync(dto.Codigo);
            ValidarHierarquiaUnica(dto);
            await ValidarEventoPaiAsync(dto.EventoPaiId);
            var statusEnum = ParseAndValidateStatus(dto.Status);
            var naturezas = await ValidarEBuscarNaturezasAsync(dto.NaturezasId);

            var evento = new Evento
            {
                Id = Guid.NewGuid().ToString(),
                Codigo = dto.Codigo,
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Endereco = dto.Endereco,
                Status = ParseAndValidateStatus(dto.Status),
                Naturezas = naturezas,
                DataEHoraDoEvento = dto.DataEHoraDoEvento,
                UsuarioCriadorId = usuario.Id,
                EventoPaiId = string.IsNullOrWhiteSpace(dto.EventoPaiId) ? null : dto.EventoPaiId,
            };
            await AssociaSubEventosNaCriacao(evento, dto);

            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();
            return evento;
        }

        public async Task AtualizarAsync(string id, CreateOrEditEventoDTO dto, Usuario usuario)
        {
            await ValidarCodigoUnicoAsync(dto.Codigo, id);
            ValidarHierarquiaUnica(dto);
            var statusEnum = ParseAndValidateStatus(dto.Status);
            var naturezasParaAssociar = await ValidarEBuscarNaturezasAsync(dto.NaturezasId); // This returns ICollection<Natureza>

            var evento = await RecuperaEventoCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(evento.UsuarioCriadorId, usuario);

            if (!string.IsNullOrWhiteSpace(dto.EventoPaiId) && dto.EventoPaiId == id)
            {
                throw new InvalidOperationException("Um evento não pode ser definido como seu próprio evento pai.");
            }

            _mapper.Map(dto, evento);
            var naturezasDtoParaPassar = _mapper.Map<ICollection<NaturezaDTO>>(naturezasParaAssociar);
            evento.Status = statusEnum;
            
            await AtualizaEventoPaiAsync(evento, dto);
            await AtualizaRelacionamentoSubEventosAsync(evento, dto);
            await AtualizaRelacionamentoNaturezasAsync(evento, naturezasDtoParaPassar);

            var acao = "Editou evento";
            AdicionaOuAtualizaHistorico(evento.Id, usuario.Id, acao);

            await _context.SaveChangesAsync();
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

            evento.isVisible = false;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EventoHistorico>> ListarHistoricoAsync(string eventoId, Usuario usuario)
        {
            if (usuario.Cargo != "ADMINISTRADOR" && usuario.Cargo != "DIRETOR")
                throw new UnauthorizedAccessException("Acesso restrito aos administradores e diretores.");

            return await _context.EventosHistoricos
                .Where(h => h.EventoId == eventoId)
                .OrderByDescending(h => h.UltimaAlteracao)
                .ToListAsync();
        }

        private async Task AtualizaEventoPaiAsync(Evento eventoParaAtualizar, CreateOrEditEventoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EventoPaiId))
            {
                eventoParaAtualizar.EventoPaiId = null;
            }
            else
            {
                await RecuperaEventoCompletoPorId(dto.EventoPaiId);
                eventoParaAtualizar.EventoPaiId = dto.EventoPaiId;
            }
        }

        private async Task AtualizaRelacionamentoSubEventosAsync(Evento eventoParaAtualizar, CreateOrEditEventoDTO dto)
        {
            var novosIds = dto.SubEventosId?.ToHashSet() ?? new HashSet<string>();

            if (novosIds.Contains(eventoParaAtualizar.Id))
            {
                throw new InvalidOperationException("Um evento não pode ser definido como seu próprio sub-evento.");
            }

            if (eventoParaAtualizar.SubEventos == null)
            {
                await _context.Entry(eventoParaAtualizar)
                    .Collection(e => e.SubEventos)
                    .LoadAsync();
            }

            var idsAtuais = eventoParaAtualizar.SubEventos!.Select(s => s.Id).ToHashSet();

            var subEventosParaRemover = eventoParaAtualizar.SubEventos
                .Where(s => !novosIds.Contains(s.Id))
                .ToList();

            foreach (var subEvento in subEventosParaRemover)
            {
                eventoParaAtualizar.SubEventos.Remove(subEvento);
                subEvento.EventoPaiId = null;
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
                    if (!string.IsNullOrWhiteSpace(subEvento.EventoPaiId) && subEvento.EventoPaiId != eventoParaAtualizar.Id)
                    {
                        throw new InvalidOperationException($"O sub-evento {subEvento.Titulo} (ID: {subEvento.Id}) já está associado a outro evento pai. Remova a associação anterior primeiro.");
                    }

                    eventoParaAtualizar.SubEventos.Add(subEvento);
                    subEvento.EventoPaiId = eventoParaAtualizar.Id;
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

        private void AdicionaOuAtualizaHistorico(string eventoId, string usuarioId, string acao)
        {
            var registroUsuarioNoHistoricoEvento = _context.EventosHistoricos.FirstOrDefault(e =>
                e.EventoId == eventoId &&
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
            var evento = await _context.Eventos
                .Include(e => e.UsuarioCriador)
                .Include(e => e.EventoPai)
                .Include(e => e.SubEventos)
                .Include(e => e.Naturezas)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null)
                throw new InvalidOperationException($"O evento com o ID '{id}' não foi encontrado.");

            if (!evento.isVisible)
                throw new InvalidOperationException($"O evento com o ID '{id}' foi deletado, entre em contato com o administrador para mais informações.");

            return evento;
        }

        private void ValidarHierarquiaUnica(CreateOrEditEventoDTO dto)
        {
            var idDoPai = dto.EventoPaiId;
            var idsDosSubEventos = dto.SubEventosId?.ToHashSet() ?? new HashSet<string>();

            if (string.IsNullOrWhiteSpace(idDoPai) || idsDosSubEventos.Count == 0)
            {
                return;
            }

            if (idsDosSubEventos.Contains(idDoPai))
            {
                throw new InvalidOperationException($"O evento pai (ID: {idDoPai}) não pode ser listado simultaneamente como um sub-evento.");
            }
        }

        private async Task AssociaSubEventosNaCriacao(Evento novoEventoPai, CreateOrEditEventoDTO dto)
        {
            if (dto.SubEventosId != null && dto.SubEventosId.Any())
            {
                var subEventos = await _context.Eventos
                    .Where(e => dto.SubEventosId.Contains(e.Id))
                    .ToListAsync();

                if (subEventos.Count != dto.SubEventosId.Count)
                {
                    var idsEncontrados = subEventos.Select(e => e.Id).ToList();
                    var idsNaoEncontrados = dto.SubEventosId.Except(idsEncontrados);
                    throw new InvalidOperationException($"Os seguintes IDs de sub-eventos não foram encontrados: {string.Join(", ", idsNaoEncontrados)}");
                }

                foreach (var subEvento in subEventos)
                {
                    subEvento.EventoPaiId = novoEventoPai.Id;
                }
            }
        }

        private EStatusEvento ParseAndValidateStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new InvalidOperationException("O status não pode ser nulo ou vazio.");
            }

            if (!Enum.TryParse<EStatusEvento>(status, true, out var statusEnum))
            {
                throw new InvalidOperationException($"O status '{status}' é inválido.");
            }

            return statusEnum;
        }

        private async Task<ICollection<Natureza>> ValidarEBuscarNaturezasAsync(ICollection<string>? naturezasId)
        {
            if (naturezasId == null || !naturezasId.Any())
            {
                return new List<Natureza>();
            }

            var naturezas = await _context.Natureza
                                   .Where(n => naturezasId.Contains(n.Id))
                                   .ToListAsync();

            var uniqueRequestedIdsCount = naturezasId.Distinct().Count();
            if (naturezas.Count != uniqueRequestedIdsCount)
            {
                var foundIds = naturezas.Select(n => n.Id).ToHashSet();
                var missingIds = naturezasId.Distinct().Where(id => !foundIds.Contains(id));

                throw new ArgumentException($"As seguintes IDs de naturezas não foram encontradas: {string.Join(", ", missingIds)}");
            }

            return naturezas;
        }

        private async Task AtualizaRelacionamentoNaturezasAsync(Evento eventoParaAtualizar, ICollection<NaturezaDTO> naturezasNoDto)
        {
            await _context.Entry(eventoParaAtualizar)
                .Collection(e => e.Naturezas)
                .LoadAsync();

            var naturezasAtuais = eventoParaAtualizar.Naturezas ?? new List<Natureza>();
            var naturezasDtoIds = naturezasNoDto?.Select(n => n.Id).ToHashSet() ?? new HashSet<string>();
            var naturezasParaRemover = naturezasAtuais.Where(n => !naturezasDtoIds.Contains(n.Id)).ToList();

            foreach (var nat in naturezasParaRemover)
            {
                naturezasAtuais.Remove(nat);
            }

            var naturezasAtuaisIds = naturezasAtuais.Select(n => n.Id).ToHashSet();
            var idsParaAdicionar = naturezasDtoIds.Where(id => !naturezasAtuaisIds.Contains(id)).ToList();

            if (idsParaAdicionar.Any())
            {
                var naturezasEntidadesParaAdicionar = await _context.Natureza
                    .Where(n => idsParaAdicionar.Contains(n.Id))
                    .ToListAsync();

                if (naturezasEntidadesParaAdicionar.Count != idsParaAdicionar.Count)
                {
                    var idsEncontrados = naturezasEntidadesParaAdicionar.Select(n => n.Id).ToHashSet();
                    var idsNaoEncontrados = idsParaAdicionar.Where(id => !idsEncontrados.Contains(id));
                    throw new InvalidOperationException($"As seguintes IDs de naturezas não foram encontradas ao tentar adicioná-las: {string.Join(", ", idsNaoEncontrados)}");
                }

                foreach (var natEntidade in naturezasEntidadesParaAdicionar)
                {
                    naturezasAtuais.Add(natEntidade);
                }
            }
            eventoParaAtualizar.Naturezas = naturezasAtuais;
        }
        private async Task ValidarEventoPaiAsync(string? eventoPaiId)
        {
            if (string.IsNullOrWhiteSpace(eventoPaiId)) 
                return;
            var paiExiste = await _context.Eventos.AnyAsync(e => e.Id == eventoPaiId);
            if (!paiExiste) 
                throw new InvalidOperationException($"O evento pai com o ID '{eventoPaiId}' não foi encontrado.");
        }
    }
}
