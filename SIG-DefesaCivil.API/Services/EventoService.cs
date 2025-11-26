using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.DTO.Eventos;
using SIG_DefesaCivil.API.Enums;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.Services
{
    public class EventoService
    {
        private readonly DefesaCivilDbContext _context;
        private readonly IMapper _mapper;
        private readonly AnexoService _anexoService;

        public EventoService(DefesaCivilDbContext context, IMapper mapper, AnexoService anexoService)
        {
            _context = context;
            _mapper = mapper;
            _anexoService = anexoService;
        }

        public async Task<EventoDetalhesDTO> DetalhesEventosPorId(string id, Usuario usuario)
        {
            var evento = await RecuperaEventoCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(evento.UsuarioCriadorId, usuario);

            string acao = "Visualizou detalhes";
            AdicionaOuAtualizaHistorico(evento.Id, usuario.Id, acao);

            // Busca os anexos genéricos associados a este evento
            var anexos = await _context.Anexos
                .Where(a => a.EntidadeId == evento.Id && a.TipoEntidade == "Evento")
                .AsNoTracking()
                .ToListAsync();

            // Mapeia o evento principal
            var eventoDto = _mapper.Map<EventoDetalhesDTO>(evento);

            // Mapeia e atribui os anexos
            eventoDto.Anexos = _mapper.Map<ICollection<AnexoDTO>>(anexos);

            await _context.SaveChangesAsync(); // Salva o histórico
            return eventoDto;
        }

        public async Task<Evento> CriarAsync(CreateOrEditEventoDTO dto, List<IFormFile>? anexos, Usuario usuario)
        {
            // --- 1. Validações ---
            await ValidarCodigoUnicoAsync(dto.Codigo);
            ValidarHierarquiaUnica(dto);
            await ValidarEventoPaiAsync(dto.EventoPaiId);
            var statusEnum = ParseAndValidateStatus(dto.Status);
            var naturezas = await ValidarEBuscarNaturezasAsync(dto.NaturezasId);

            // --- 2. Mapeamento e Criação ---
            var evento = _mapper.Map<Evento>(dto); // Mapeia campos simples

            // Define propriedades gerenciadas manualmente
            evento.Id = Guid.NewGuid().ToString();
            evento.UsuarioCriadorId = usuario.Id;
            evento.EventoPaiId = string.IsNullOrWhiteSpace(dto.EventoPaiId) ? null : dto.EventoPaiId;
            evento.Naturezas = naturezas;
            evento.isVisible = true; // Valor padrão definido na entidade

            await AssociaSubEventosNaCriacao(evento, dto);

            // --- 3. Salva o Evento Principal ---
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync(); // Salva o evento para que ele tenha um ID

            // --- 4. Salva Anexos (agora que o evento.Id existe) ---
            if (anexos != null && anexos.Any())
            {
                foreach (var arquivo in anexos)
                {
                    // Delega para o AnexoService, que salva no Drive e no DB
                    await _anexoService.SalvarAnexoAsync(arquivo, evento.Id, "Evento");
                }
                await _context.SaveChangesAsync(); // Salva os anexos
            }

            return evento;
        }

        public async Task AtualizarAsync(string id, CreateOrEditEventoDTO dto, List<IFormFile>? anexosNovos, List<string>? anexosParaRemoverIds, Usuario usuario)
        {
            // --- 1. Validações ---
            await ValidarCodigoUnicoAsync(dto.Codigo, id);
            ValidarHierarquiaUnica(dto);
            var statusEnum = ParseAndValidateStatus(dto.Status);
            var naturezasParaAssociar = await ValidarEBuscarNaturezasAsync(dto.NaturezasId);

            // --- 2. Busca da Entidade ---
            var evento = await RecuperaEventoCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(evento.UsuarioCriadorId, usuario);

            if (!string.IsNullOrWhiteSpace(dto.EventoPaiId) && dto.EventoPaiId == id)
            {
                throw new InvalidOperationException("Um evento não pode ser definido como seu próprio evento pai.");
            }

            // --- 3. Mapeamento e Atualização de Relações ---
            _mapper.Map(dto, evento); // Atualiza campos simples (Titulo, Descricao, etc.)

            await AtualizaEventoPaiAsync(evento, dto);
            await AtualizaRelacionamentoSubEventosAsync(evento, dto);

            // CORRIGIDO: Passa a coleção de ENTIDADES, não de DTOs
            await AtualizaRelacionamentoNaturezasAsync(evento, naturezasParaAssociar);

            // --- 4. Gerenciamento de Anexos ---
            // Remover anexos
            if (anexosParaRemoverIds != null && anexosParaRemoverIds.Any())
            {
                foreach (var anexoId in anexosParaRemoverIds)
                {
                    // Verifica se o anexo pertence a este evento antes de excluir
                    var anexo = await _context.Anexos.FirstOrDefaultAsync(a => a.Id == anexoId && a.EntidadeId == evento.Id && a.TipoEntidade == "Evento");
                    if (anexo != null)
                    {
                        await _anexoService.ExcluirAnexoAsync(anexo.Id);
                    }
                }
            }
            // Adicionar novos anexos
            if (anexosNovos != null && anexosNovos.Any())
            {
                foreach (var arquivo in anexosNovos)
                {
                    await _anexoService.SalvarAnexoAsync(arquivo, evento.Id, "Evento");
                }
            }

            // --- 5. Salvar ---
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

            if (evento.SubEventos != null && evento.SubEventos.Any(se => se.isVisible))
            {
                throw new InvalidOperationException("Não é possível excluir este evento pois ele possui sub-eventos visíveis associados. Remova ou reatribua os sub-eventos primeiro.");
            }

            // Soft Delete
            evento.isVisible = false;
            var acao = "Deletou evento";
            AdicionaOuAtualizaHistorico(evento.Id, usuario.Id, acao);

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
        public async Task<ICollection<AnexoDTO>> GetAnexosDTOByEventoIdAsync(string eventoId)
        {
            var anexos = await _context.Anexos
                .Where(a => a.EntidadeId == eventoId && a.TipoEntidade == "Evento")
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<ICollection<AnexoDTO>>(anexos);
        }

        private async Task AtualizaEventoPaiAsync(Evento eventoParaAtualizar, CreateOrEditEventoDTO dto)
        {
            await ValidarEventoPaiAsync(dto.EventoPaiId);
            eventoParaAtualizar.EventoPaiId = string.IsNullOrWhiteSpace(dto.EventoPaiId) ? null : dto.EventoPaiId;
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
                .Include(e => e.EventoPai).ThenInclude(p => p.UsuarioCriador)
                .Include(e => e.SubEventos).ThenInclude(s => s.UsuarioCriador)
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

        private async Task AtualizaRelacionamentoNaturezasAsync(Evento eventoParaAtualizar, ICollection<Natureza> naturezasParaAssociar)
        {
            await _context.Entry(eventoParaAtualizar)
                .Collection(e => e.Naturezas)
                .LoadAsync();

            var naturezasAtuais = eventoParaAtualizar.Naturezas ?? new List<Natureza>();
            var naturezasDtoIds = naturezasParaAssociar.Select(n => n.Id).ToHashSet();

            var naturezasParaRemover = naturezasAtuais.Where(n => !naturezasDtoIds.Contains(n.Id)).ToList();
            foreach (var nat in naturezasParaRemover)
            {
                naturezasAtuais.Remove(nat);
            }

            var naturezasAtuaisIds = naturezasAtuais.Select(n => n.Id).ToHashSet();
            var naturezasParaAdicionar = naturezasParaAssociar.Where(n => !naturezasAtuaisIds.Contains(n.Id)).ToList();
            foreach (var nat in naturezasParaAdicionar)
            {
                naturezasAtuais.Add(nat);
            }
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
