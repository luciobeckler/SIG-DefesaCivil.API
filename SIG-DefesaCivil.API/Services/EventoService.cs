using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.DTO.Eventos.SIG_DefesaCivil.API.DTO.EventoDTO;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.Services
{
    public class EventoService
    {
        private readonly DefesaCivilDbContext _context;
        private readonly IMapper _mapper;
        private readonly AnexoService _anexoService; // Injetado para gerenciar arquivos

        public EventoService(DefesaCivilDbContext context, IMapper mapper, AnexoService anexoService)
        {
            _context = context;
            _mapper = mapper;
            _anexoService = anexoService;
        }

        // --- MÉTODOS PÚBLICOS ---

        /// <summary>
        /// Lista os "cartões" (previews) de eventos visíveis.
        /// </summary>
        public async Task<IEnumerable<EventoPreviewDTO>> ListarPreviewEventosAsync()
        {
            // Usamos ProjectTo para performance, o AutoMapper Profile deve estar configurado
            return await _context.Eventos
                .Where(e => e.isVisible == true)
                .AsNoTracking()
                .ProjectTo<EventoPreviewDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            /* // Alternativa com Mapeamento Manual (se o ProjectTo falhar):
            var allEventos = await _context.Eventos
                .Where(e => e.isVisible == true)
                .Include(e => e.UsuarioCriador)
                .Include(e => e.Stage) // Necessário para StageName
                .Include(e => e.Naturezas)
                .AsNoTracking()
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<EventoPreviewDTO>>(allEventos);
            */
        }

        /// <summary>
        /// Busca os detalhes completos de um evento (para leitura).
        /// </summary>
        public async Task<EventoDetalhesDTO> DetalhesEventosPorId(string id, Usuario usuario)
        {
            var evento = await RecuperaEventoCompletoPorIdAsync(id); // Método de Leitura
            VerificaSeUsuarioPossuiPermissao(evento.UsuarioCriadorId, usuario);

            var eventoDto = _mapper.Map<EventoDetalhesDTO>(evento);

            // Busca e anexa os DTOs de Anexos
            eventoDto.Anexos = await GetAnexosDTOByEventoIdAsync(evento.Id);

            string acao = "Visualizou detalhes";
            AdicionaOuAtualizaHistorico(evento.Id, usuario.Id, acao);
            await _context.SaveChangesAsync();
            return eventoDto;
        }

        /// <summary>
        /// Cria um novo evento (cartão) em um stage.
        /// </summary>
        public async Task<Evento> CriarAsync(CreateEventoDTO dto, List<IFormFile>? anexos, Usuario usuario)
        {
            // --- 1. Validações ---
            await ValidarCodigoUnicoAsync(dto.Codigo);
            ValidarHierarquiaUnica(dto.EventoPaiId, dto.SubEventosId);
            await ValidarEventoPaiAsync(dto.EventoPaiId);
            await ValidarStageAsync(dto.StageId); // Valida o Stage
            var naturezas = await ValidarEBuscarNaturezasAsync(dto.NaturezasId);

            // --- 2. Mapeamento e Criação ---
            var evento = _mapper.Map<Evento>(dto); // Mapeia campos simples (Titulo, Descricao, StageId, etc.)

            evento.Id = Guid.NewGuid().ToString();
            evento.UsuarioCriadorId = usuario.Id;
            evento.Naturezas = naturezas;
            evento.isVisible = true;
            // O EventoPaiId é mapeado pelo AutoMapper se existir no DTO base

            await AssociaSubEventosNaCriacao(evento, dto.SubEventosId);

            // --- 3. Salva Evento Principal ---
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync(); // Salva para obter o evento.Id

            // --- 4. Salva Anexos ---
            if (anexos != null && anexos.Any())
            {
                foreach (var arquivo in anexos)
                {
                    await _anexoService.SalvarAnexoAsync(arquivo, evento.Id, "Evento");
                }
                await _context.SaveChangesAsync(); // Salva os anexos
            }

            var acao = "Criou evento";
            AdicionaOuAtualizaHistorico(evento.Id, usuario.Id, acao);
            await _context.SaveChangesAsync();

            return evento;
        }

        /// <summary>
        /// Atualiza os dados de um evento (cartão). Não move o evento.
        /// </summary>
        public async Task AtualizarAsync(string id, UpdateEventoDTO dto, List<IFormFile>? anexosNovos, List<string>? anexosParaRemoverIds, Usuario usuario)
        {
            // --- 1. Validações ---
            await ValidarCodigoUnicoAsync(dto.Codigo, id);
            ValidarHierarquiaUnica(dto.EventoPaiId, dto.SubEventosId);
            var naturezasParaAssociar = await ValidarEBuscarNaturezasAsync(dto.NaturezasId);

            // --- 2. Busca da Entidade (RASTREADA) ---
            var evento = await RecuperaEventoParaAtualizarAsync(id); // Método de Escrita
            VerificaSeUsuarioPossuiPermissao(evento.UsuarioCriadorId, usuario);

            if (!string.IsNullOrWhiteSpace(dto.EventoPaiId) && dto.EventoPaiId == id)
                throw new InvalidOperationException("Um evento não pode ser definido como seu próprio evento pai.");

            // --- 3. Mapeamento e Atualização ---
            _mapper.Map(dto, evento); // Atualiza (Titulo, Descricao, isVisible, etc.)

            await AtualizaEventoPaiAsync(evento, dto.EventoPaiId);
            await AtualizaRelacionamentoSubEventosAsync(evento, dto.SubEventosId);

            // CORRIGIDO: Passa a coleção de ENTIDADES
            await AtualizaRelacionamentoNaturezasAsync(evento, naturezasParaAssociar);

            await AtualizaRelacionamentoAnexosAsync(evento.Id, anexosNovos, anexosParaRemoverIds);

            // --- 4. Salvar ---
            var acao = "Editou evento";
            AdicionaOuAtualizaHistorico(evento.Id, usuario.Id, acao);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Move um evento (cartão) para um novo Stage (coluna).
        /// </summary>
        public async Task MoverAsync(string eventoId, string novoStageId, Usuario usuario)
        {
            // Busca o evento RASTREADO
            var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == eventoId);
            if (evento == null)
                throw new KeyNotFoundException("Evento (cartão) não encontrado.");

            if (!evento.isVisible)
                throw new InvalidOperationException("Não é possível mover um evento deletado.");

            VerificaSeUsuarioPossuiPermissao(evento.UsuarioCriadorId, usuario);
            await ValidarStageAsync(novoStageId);

            var stageAntigoId = evento.StageId;
            if (stageAntigoId == novoStageId) return; // Não faz nada se já está no stage

            evento.StageId = novoStageId;

            var acao = $"Moveu cartão do stage {stageAntigoId} para {novoStageId}";
            AdicionaOuAtualizaHistorico(evento.Id, usuario.Id, acao);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Realiza o "Soft Delete" de um evento.
        /// </summary>
        public async Task DeletarAsync(string id, Usuario usuario)
        {
            var evento = await _context.Eventos
                .Include(e => e.SubEventos.Where(s => s.isVisible))
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null)
                throw new InvalidOperationException($"O evento com o ID '{id}' não foi encontrado.");

            bool podeDeletar = usuario.Cargo.ToUpper() == "ADMINISTRADOR" || usuario.Cargo.ToUpper() == "DIRETOR";
            if (!podeDeletar)
                throw new UnauthorizedAccessException("Você não tem permissão para excluir eventos.");

            if (evento.SubEventos != null && evento.SubEventos.Any())
            {
                throw new InvalidOperationException("Não é possível excluir este evento pois ele possui sub-eventos visíveis associados.");
            }

            evento.isVisible = false;
            var acao = "Deletou evento";
            AdicionaOuAtualizaHistorico(evento.Id, usuario.Id, acao);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Lista os anexos de um evento específico.
        /// </summary>
        public async Task<ICollection<AnexoDTO>> GetAnexosDTOByEventoIdAsync(string eventoId)
        {
            return await _context.Anexos
                .Where(a => a.EntidadeId == eventoId && a.TipoEntidade == "Evento")
                .AsNoTracking()
                .ProjectTo<AnexoDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        /// <summary>
        /// Lista o histórico de um evento.
        /// </summary>
        public async Task<IEnumerable<EventoHistorico>> ListarHistoricoAsync(string eventoId, Usuario usuario)
        {
            // ... (código existente, apenas adicione a permissão de visualização de histórico)
            var evento = await _context.Eventos.FindAsync(eventoId);
            if (evento == null) throw new KeyNotFoundException("Evento não encontrado.");

            // Permite que o criador veja o histórico, mesmo que não seja admin
            VerificaSeUsuarioPossuiPermissao(evento.UsuarioCriadorId, usuario);

            // Ou, se a regra for "só admin vê histórico":
            // if (usuario.Cargo != "ADMINISTRADOR" && usuario.Cargo != "DIRETOR")
            //    throw new UnauthorizedAccessException("Acesso restrito aos administradores e diretores.");

            return await _context.EventosHistoricos
                .Where(h => h.EventoId == eventoId)
                .OrderByDescending(h => h.UltimaAlteracao)
                .AsNoTracking()
                .ToListAsync();
        }

        // --- MÉTODOS PRIVADOS AUXILIARES ---

        /// <summary>
        /// Recupera um evento completo para LEITURA (Não Rastreado).
        /// </summary>
        private async Task<Evento> RecuperaEventoCompletoPorIdAsync(string id)
        {
            var evento = await _context.Eventos
                .AsNoTracking() // Otimizado para leitura
                .Include(e => e.UsuarioCriador)
                .Include(e => e.EventoPai).ThenInclude(p => p.UsuarioCriador)
                .Include(e => e.EventoPai).ThenInclude(p => p.Stage)
                .Include(e => e.SubEventos.Where(s => s.isVisible)).ThenInclude(s => s.UsuarioCriador)
                .Include(e => e.SubEventos.Where(s => s.isVisible)).ThenInclude(s => s.Stage)
                .Include(e => e.Naturezas)
                .Include(e => e.Stage) // Inclui o Stage atual
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null)
                throw new InvalidOperationException($"O evento com o ID '{id}' não foi encontrado.");

            if (!evento.isVisible)
                throw new InvalidOperationException($"O evento com o ID '{id}' foi deletado...");

            return evento;
        }

        /// <summary>
        /// Recupera um evento completo para ATUALIZAÇÃO (Rastreado).
        /// </summary>
        private async Task<Evento> RecuperaEventoParaAtualizarAsync(string id)
        {
            var evento = await _context.Eventos
               .Include(e => e.SubEventos)
               .Include(e => e.Naturezas)
               .Include(e => e.UsuarioCriador) // Para verificação de permissão
               .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null)
                throw new InvalidOperationException($"O evento com o ID '{id}' não foi encontrado.");

            if (!evento.isVisible)
                throw new InvalidOperationException($"Não é possível atualizar um evento deletado.");

            return evento;
        }

        private async Task AtualizaEventoPaiAsync(Evento evento, string? eventoPaiId)
        {
            await ValidarEventoPaiAsync(eventoPaiId);
            evento.EventoPaiId = string.IsNullOrWhiteSpace(eventoPaiId) ? null : eventoPaiId;
        }

        private async Task AtualizaRelacionamentoSubEventosAsync(Evento evento, ICollection<string>? subEventosId)
        {
            var novosIds = subEventosId?.ToHashSet() ?? new HashSet<string>();
            if (novosIds.Contains(evento.Id))
                throw new InvalidOperationException("Um evento não pode ser seu próprio sub-evento.");

            await _context.Entry(evento).Collection(e => e.SubEventos).LoadAsync();
            var idsAtuais = evento.SubEventos!.Select(s => s.Id).ToHashSet();

            // Remover
            var subEventosParaRemover = evento.SubEventos.Where(s => !novosIds.Contains(s.Id)).ToList();
            foreach (var sub in subEventosParaRemover)
                sub.EventoPaiId = null;

            // Adicionar
            var idsParaAdicionar = novosIds.Where(id => !idsAtuais.Contains(id)).ToList();
            if (idsParaAdicionar.Any())
            {
                var subEventosParaAdicionar = await _context.Eventos
                    .Where(e => idsParaAdicionar.Contains(e.Id))
                    .ToListAsync();

                if (subEventosParaAdicionar.Count != idsParaAdicionar.Count)
                    throw new InvalidOperationException("Um ou mais sub-eventos não foram encontrados.");

                foreach (var sub in subEventosParaAdicionar)
                    sub.EventoPaiId = evento.Id;
            }
        }

        private async Task AtualizaRelacionamentoNaturezasAsync(Evento evento, ICollection<Natureza> naturezasParaAssociar)
        {
            await _context.Entry(evento).Collection(e => e.Naturezas).LoadAsync();
            var naturezasAtuais = evento.Naturezas ?? new List<Natureza>();

            var naturezasParaAssociarIds = naturezasParaAssociar.Select(n => n.Id).ToHashSet();

            // Remover
            var naturezasParaRemover = naturezasAtuais.Where(n => !naturezasParaAssociarIds.Contains(n.Id)).ToList();
            foreach (var nat in naturezasParaRemover)
            {
                naturezasAtuais.Remove(nat);
            }

            // Adicionar
            var naturezasAtuaisIds = naturezasAtuais.Select(n => n.Id).ToHashSet();
            var naturezasParaAdicionar = naturezasParaAssociar.Where(n => !naturezasAtuaisIds.Contains(n.Id)).ToList();
            foreach (var nat in naturezasParaAdicionar)
            {
                naturezasAtuais.Add(nat);
            }
        }

        private async Task AtualizaRelacionamentoAnexosAsync(string eventoId, List<IFormFile>? anexosNovos, List<string>? anexosParaRemoverIds)
        {
            if (anexosParaRemoverIds != null && anexosParaRemoverIds.Any())
            {
                foreach (var anexoId in anexosParaRemoverIds)
                {
                    var anexo = await _context.Anexos.FirstOrDefaultAsync(a => a.Id == anexoId && a.EntidadeId == eventoId && a.TipoEntidade == "Evento");
                    if (anexo != null)
                        await _anexoService.ExcluirAnexoAsync(anexo.Id);
                }
            }
            if (anexosNovos != null && anexosNovos.Any())
            {
                foreach (var arquivo in anexosNovos)
                {
                    await _anexoService.SalvarAnexoAsync(arquivo, eventoId, "Evento");
                }
            }
        }

        // --- Validações ---

        private async Task ValidarStageAsync(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId))
                throw new ArgumentException("O StageId (coluna) é obrigatório.");

            var stageExiste = await _context.Stages.AnyAsync(s => s.Id == stageId);
            if (!stageExiste)
                throw new InvalidOperationException($"O Stage (coluna) com o ID '{stageId}' não foi encontrado.");
        }

        private void VerificaSeUsuarioPossuiPermissao(string criadorId, Usuario usuario)
        {
            var temPermissao = usuario.Cargo.ToUpper() == "ADMINISTRADOR"
                || usuario.Cargo.ToUpper() == "DIRETOR"
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

        private void ValidarHierarquiaUnica(string? eventoPaiId, ICollection<string>? subEventosId)
        {
            if (string.IsNullOrWhiteSpace(eventoPaiId) || subEventosId == null || !subEventosId.Any())
                return;
            if (subEventosId.Contains(eventoPaiId))
                throw new InvalidOperationException($"O evento pai (ID: {eventoPaiId}) não pode ser listado simultaneamente como um sub-evento.");
        }

        private async Task AssociaSubEventosNaCriacao(Evento novoEventoPai, ICollection<string>? subEventosId)
        {
            if (subEventosId == null || !subEventosId.Any()) return;
            var subEventos = await _context.Eventos
                .Where(e => subEventosId.Contains(e.Id))
                .ToListAsync();

            if (subEventos.Count != subEventosId.Count)
            {
                var idsEncontrados = subEventos.Select(e => e.Id).ToHashSet();
                var idsNaoEncontrados = subEventosId.Where(id => !idsEncontrados.Contains(id));
                throw new InvalidOperationException($"Os seguintes IDs de sub-eventos não foram encontrados: {string.Join(", ", idsNaoEncontrados)}");
            }

            foreach (var subEvento in subEventos)
                subEvento.EventoPaiId = novoEventoPai.Id;
        }

        private async Task<ICollection<Natureza>> ValidarEBuscarNaturezasAsync(ICollection<string>? naturezasId)
        {
            if (naturezasId == null || !naturezasId.Any())
            {
                return new List<Natureza>();
            }

            var naturezas = await _context.Naturezas
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