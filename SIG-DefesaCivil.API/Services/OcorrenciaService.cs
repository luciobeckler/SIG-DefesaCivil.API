using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Data.DTO.Ocorrencia;
using SIG_DefesaCivil.API.Enums;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Ocorrencia;

namespace SIG_DefesaCivil.API.Services
{
    public class OcorrenciaService
    {
        private readonly DefesaCivilDbContext _context;
        private readonly IMapper _mapper;
        private readonly AnexoService _anexoService;
        private readonly EtapaService _etapaService;
        private readonly UserManager<Usuario> _userManager;

        public OcorrenciaService(
            DefesaCivilDbContext context,
            IMapper mapper,
            AnexoService anexoService,
            EtapaService etapaService,
            UserManager<Usuario> userManager
            )
        {
            _context = context;
            _mapper = mapper;
            _anexoService = anexoService;
            _etapaService = etapaService;
            _userManager = userManager;
        }

        public async Task<Ocorrencia> GetOcorrenciaPreviewById(string id)
        {
            var ocorrencia = await _context.Ocorrencia
                .FirstOrDefaultAsync(e => e.Id.Equals(id));

            if (ocorrencia == null)
            {
                throw new KeyNotFoundException("Ocorrencia não encontrado");
            }

            return ocorrencia;
        }

        public async Task<OcorrenciaDetalhesDTO> OcorrenciaDetalheById(string id, Usuario usuario)
        {
            var ocorrencia = await RecuperaOcorrenciaCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(ocorrencia.UsuarioCriadorId, usuario);

            string acao = "Visualizou detalhes";
            AdicionaOuAtualizaHistorico(ocorrencia.Id, usuario.Id, acao);

            // Busca os anexos genéricos associados a este ocorrencia
            var anexos = await _context.Anexos
                .Where(a => a.EntidadeId == ocorrencia.Id && a.TipoEntidade == "Ocorrencia")
                .AsNoTracking()
                .ToListAsync();

            // Mapeia o ocorrencia principal
            var ocorrenciaDto = _mapper.Map<OcorrenciaDetalhesDTO>(ocorrencia);

            // Mapeia e atribui os anexos
            ocorrenciaDto.Anexos = _mapper.Map<List<AnexoDTO>>(anexos);

            await _context.SaveChangesAsync(); // Salva o histórico
            return ocorrenciaDto;
        }

        public async Task<Ocorrencia> CriarAsync(Usuario usuario, string quadroId, CreateOrEditOcorrenciaDTO dto)
        {
            // --- 1. Validações Prévias ---
            await ValidarOcorrenciaPaiAsync(dto.OcorrenciaPaiId);
            var naturezas = await ValidarEBuscarNaturezasAsync(dto.NaturezasId);

            var anoProtocolo = dto.DataEHoraDoOcorrido.Value.Year;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string novoNumeroProtocolo;

                // Busca a última ocorrência APENAS daquele ano específico
                // Ordenamos por DataCriacao (ou Id) decrescente para pegar o último inserido
                var ultimaOcorrenciaDoAno = await _context.Ocorrencia
                    .Where(x => x.Numero.StartsWith($"{anoProtocolo}-"))
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => x.Numero)
                    .FirstOrDefaultAsync();

                if (ultimaOcorrenciaDoAno == null)
                {
                    novoNumeroProtocolo = $"{anoProtocolo}-1";
                }
                else
                {
                    // Separa "2026-15" em ["2026", "15"]
                    var partes = ultimaOcorrenciaDoAno.Split('-');

                    // Garante que a segunda parte é um número
                    if (partes.Length > 1 && int.TryParse(partes[1], out int sequencialAtual))
                    {
                        novoNumeroProtocolo = $"{anoProtocolo}-{sequencialAtual + 1}";
                    }
                    else
                    {
                        // Fallback caso o banco tenha algum dado sujo
                        novoNumeroProtocolo = $"{anoProtocolo}-1";
                    }
                }

                // --- 3. Mapeamento e Criação ---
                var ocorrencia = _mapper.Map<Ocorrencia>(dto);

                ocorrencia.Id = Guid.NewGuid().ToString();
                ocorrencia.Numero = novoNumeroProtocolo; // Atribui o número gerado
                ocorrencia.UsuarioCriadorId = usuario.Id;
                ocorrencia.OcorrenciaPaiId = string.IsNullOrWhiteSpace(dto.OcorrenciaPaiId) ? null : dto.OcorrenciaPaiId;
                ocorrencia.Naturezas = naturezas;
                ocorrencia.isVisible = true;

                await AssociaSubOcorrenciasNaCriacao(ocorrencia, dto);

                _context.Ocorrencia.Add(ocorrencia);

                // Adiciona à etapa
                await _etapaService.AdicionaOcorrenciaNaPrimeiraEtapaAsync(usuario, ocorrencia, quadroId);

                await _context.SaveChangesAsync();

                // Confirma a transação
                await transaction.CommitAsync();

                return ocorrencia;
            }
            catch (Exception)
            {
                // Se der erro, desfaz tudo
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AtualizarAsync(string id, CreateOrEditOcorrenciaDTO dto, Usuario usuario)
        {
            var naturezasParaAssociar = await ValidarEBuscarNaturezasAsync(dto.NaturezasId);

            // --- 2. Busca da Entidade ---
            var ocorrencia = await RecuperaOcorrenciaCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(ocorrencia.UsuarioCriadorId, usuario);

            if (!string.IsNullOrWhiteSpace(dto.OcorrenciaPaiId) && dto.OcorrenciaPaiId == id)
            {
                throw new InvalidOperationException("Um ocorrencia não pode ser definido como seu próprio ocorrencia pai.");
            }

            // --- 3. Mapeamento e Atualização de Relações ---
            _mapper.Map(dto, ocorrencia); // Atualiza campos simples (Titulo, Descricao, etc.)

            await AtualizaOcorrenciaPaiAsync(ocorrencia, dto);
            await AtualizaRelacionamentoSubOcorrenciasAsync(ocorrencia, dto);

            // CORRIGIDO: Passa a coleção de ENTIDADES, não de DTOs
            await AtualizaRelacionamentoNaturezasAsync(ocorrencia, naturezasParaAssociar);

            // --- 5. Salvar ---
            var acao = "Editou ocorrencia";
            AdicionaOuAtualizaHistorico(ocorrencia.Id, usuario.Id, acao);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(string id, Usuario usuario)
        {
            var ocorrencia = await _context.Ocorrencia
                .Include(e => e.SubOcorrencias)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ocorrencia == null)
            {
                throw new InvalidOperationException($"O ocorrencia com o ID '{id}' não foi encontrado.");
            }

            bool podeDeletar = usuario.Cargo == nameof(ECargos.Administrador) || usuario.Cargo == nameof(ECargos.Diretor);
            if (!podeDeletar)
                throw new UnauthorizedAccessException("Você não tem permissão para excluir ocorrencias.");

            if (ocorrencia.SubOcorrencias != null && ocorrencia.SubOcorrencias.Any(se => se.isVisible))
            {
                throw new InvalidOperationException("Não é possível excluir este ocorrencia pois ele possui sub-ocorrencias visíveis associados. Remova ou reatribua os sub-ocorrencias primeiro.");
            }

            // Soft Delete
            ocorrencia.isVisible = false;
            var acao = "Deletou ocorrencia";
            AdicionaOuAtualizaHistorico(ocorrencia.Id, usuario.Id, acao);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OcorrenciaHistorico>> ListarHistoricoAsync(string ocorrenciaId, Usuario usuario)
        {
            if (usuario.Cargo != nameof(ECargos.Administrador) && usuario.Cargo != nameof(ECargos.Diretor))
                throw new UnauthorizedAccessException("Acesso restrito aos administradores e diretores.");

            var historicos = await _context.OcorrenciasHistoricos
                .Include(h => h.Usuario)
                .Where(h => h.OcorrenciaId == ocorrenciaId)
                .ToListAsync();

            var historicosOrdenados = historicos
                .OrderBy(h => h.Usuario.Nome)
                .Select(h =>
                {
                    h.Horarios = h.Horarios.OrderByDescending(data => data).ToList();
                    return h;
                })
                .ToList();

            return historicosOrdenados;
        }

        public async Task<List<AnexoDTO>> GetAnexosDTOByOcorrenciaIdAsync(string ocorrenciaId)
        {
            var anexos = await _context.Anexos
                .Where(a => a.EntidadeId == ocorrenciaId && a.TipoEntidade == "Ocorrencia")
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<AnexoDTO>>(anexos);
        }

        public async Task TransicionaOcorrencia(Usuario usuario, string ocorrenciaId, string etapaAtualId, string etapaDestinoId)
        {
            var ocorrencia = await _context.Ocorrencia
                .FirstOrDefaultAsync(o => o.Id == ocorrenciaId);
            if (ocorrencia == null)
                throw new KeyNotFoundException("Ocorrencia não encontrada");

            var etapaAtual = await _etapaService.GetEtapaById(etapaAtualId);
            var etapaDestino = await _etapaService.GetEtapaById(etapaDestinoId);

            _etapaService.VerificaRegrasDeTransicao(usuario, ocorrencia, etapaAtual, etapaDestino);

            etapaAtual.Ocorrencias.Remove(ocorrencia);
            etapaDestino.Ocorrencias.Add(ocorrencia);
            ocorrencia.DataEntradaNaFaseAtual = DateTime.Now;

            await _context.SaveChangesAsync();
        }
        public async Task ProcessarMovimentacoesAutomaticas()
        {
            var usuarioSistema = await _userManager.FindByEmailAsync("sistema@admin.com");

            var etapasComPrazo = await _context.Etapas
                .Where(e => e.MaxTempoNaEtapa != null && e.MaxTempoNaEtapa != TimeSpan.MaxValue)
                .Include(e => e.Ocorrencias)
                .ToListAsync();

            foreach (var etapa in etapasComPrazo)
            {
                if (etapa.EtapasDestinoId == null || !etapa.EtapasDestinoId.Any())
                {
                    Console.WriteLine($"[AUTO SKIP] Etapa '{etapa.Nome}' tem prazo mas não tem etapa de destino configurada.");
                    continue;
                }

                // Define o destino padrão (Assumimos o primeiro da lista como o fluxo natural)
                var idEtapaDestino = etapa.EtapasDestinoId.First();

                var prazo = etapa.MaxTempoNaEtapa.Value;

                var ocorrenciasVencidas = etapa.Ocorrencias
                    .Where(o => o.DataEntradaNaFaseAtual.HasValue &&
                                o.DataEntradaNaFaseAtual.Value.Add(prazo) <= DateTime.Now)
                    .ToList();

                if (!ocorrenciasVencidas.Any()) continue;

                Console.WriteLine($"[AUTO] Processando etapa '{etapa.Nome}': {ocorrenciasVencidas.Count} vencidas.");

                foreach (var ocorrencia in ocorrenciasVencidas)
                {
                    try
                    {
                        await TransicionaOcorrencia(usuarioSistema, ocorrencia.Id, etapa.Id, idEtapaDestino);
                        Console.WriteLine($"   -> Ocorrência {ocorrencia.Numero} movida para próxima etapa.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   -> [ERRO] Falha ao mover {ocorrencia.Numero}: {ex.Message}");
                    }
                }
            }
        }
        private async Task AtualizaOcorrenciaPaiAsync(Ocorrencia ocorrenciaParaAtualizar, CreateOrEditOcorrenciaDTO dto)
        {
            await ValidarOcorrenciaPaiAsync(dto.OcorrenciaPaiId);
            ocorrenciaParaAtualizar.OcorrenciaPaiId = string.IsNullOrWhiteSpace(dto.OcorrenciaPaiId) ? null : dto.OcorrenciaPaiId;
        }

        private async Task AtualizaRelacionamentoSubOcorrenciasAsync(Ocorrencia ocorrenciaParaAtualizar, CreateOrEditOcorrenciaDTO dto)
        {
            var novosIds = dto.SubOcorrenciasId?.ToHashSet() ?? new HashSet<string>();

            if (novosIds.Contains(ocorrenciaParaAtualizar.Id))
            {
                throw new InvalidOperationException("Um ocorrencia não pode ser definido como seu próprio sub-ocorrencia.");
            }

            if (ocorrenciaParaAtualizar.SubOcorrencias == null)
            {
                await _context.Entry(ocorrenciaParaAtualizar)
                    .Collection(e => e.SubOcorrencias)
                    .LoadAsync();
            }

            var idsAtuais = ocorrenciaParaAtualizar.SubOcorrencias!.Select(s => s.Id).ToHashSet();

            var subOcorrenciasParaRemover = ocorrenciaParaAtualizar.SubOcorrencias
                .Where(s => !novosIds.Contains(s.Id))
                .ToList();

            foreach (var subOcorrencia in subOcorrenciasParaRemover)
            {
                ocorrenciaParaAtualizar.SubOcorrencias.Remove(subOcorrencia);
                subOcorrencia.OcorrenciaPaiId = null;
            }

            var idsParaAdicionar = novosIds.Where(id => !idsAtuais.Contains(id)).ToList();

            if (idsParaAdicionar.Any())
            {
                var subOcorrenciaParaAdicionar = await _context.Ocorrencia
                    .Where(e => idsParaAdicionar.Contains(e.Id))
                    .ToListAsync();

                if (subOcorrenciaParaAdicionar.Count != idsParaAdicionar.Count)
                {
                    var idsEncontrados = subOcorrenciaParaAdicionar.Select(e => e.Id).ToList();
                    var idsNaoEncontrados = idsParaAdicionar.Except(idsEncontrados);

                    throw new InvalidOperationException($"Os seguintes IDs de sub-ocorrencias não foram encontrados: {string.Join(", ", idsNaoEncontrados)}");
                }

                foreach (var subOcorrencia in subOcorrenciaParaAdicionar)
                {
                    if (!string.IsNullOrWhiteSpace(subOcorrencia.OcorrenciaPaiId) && subOcorrencia.OcorrenciaPaiId != ocorrenciaParaAtualizar.Id)
                    {
                        throw new InvalidOperationException($"A sub-ocorrencia {subOcorrencia.Numero} (ID: {subOcorrencia.Id}) já está associado a outra ocorrência pai. Remova a associação anterior primeiro.");
                    }

                    ocorrenciaParaAtualizar.SubOcorrencias.Add(subOcorrencia);
                    subOcorrencia.OcorrenciaPaiId = ocorrenciaParaAtualizar.Id;
                }
            }
        }

        private void VerificaSeUsuarioPossuiPermissao(string criadorId, Usuario usuario)
        {
            var temPermissao = usuario.Cargo == nameof(ECargos.Administrador)
                    || usuario.Cargo == nameof(ECargos.Diretor)
                    || usuario.Id == criadorId;

            if (!temPermissao)
            {
                throw new UnauthorizedAccessException("Você não possui permissão para editar este ocorrencia.");
            }
        }

        private void AdicionaOuAtualizaHistorico(string ocorrenciaId, string usuarioId, string acao)
        {
            var registroUsuarioNoHistoricoOcorrencia = _context.OcorrenciasHistoricos.FirstOrDefault(e =>
                e.OcorrenciaId == ocorrenciaId &&
                e.UsuarioId == usuarioId &&
                e.Acao == acao);

            if (registroUsuarioNoHistoricoOcorrencia == null)
            {
                _context.OcorrenciasHistoricos.Add(new OcorrenciaHistorico
                {
                    OcorrenciaId = ocorrenciaId,
                    UsuarioId = usuarioId,
                    Acao = acao,
                    Horarios = new List<DateTime> { DateTime.Now }
                });
            }
            else
            {
                registroUsuarioNoHistoricoOcorrencia.Horarios.Add(DateTime.Now);
            }
        }
        private async Task<Ocorrencia> RecuperaOcorrenciaCompletoPorId(string id)
        {
            var ocorrencia = await _context.Ocorrencia
                .Include(e => e.UsuarioCriador)
                .Include(e => e.OcorrenciaPai).ThenInclude(p => p.UsuarioCriador)
                .Include(e => e.SubOcorrencias).ThenInclude(s => s.UsuarioCriador)
                .Include(e => e.Naturezas)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ocorrencia == null)
                throw new InvalidOperationException($"O ocorrencia com o ID '{id}' não foi encontrado.");

            if (!ocorrencia.isVisible)
                throw new InvalidOperationException($"O ocorrencia com o ID '{id}' foi deletado, entre em contato com o administrador para mais informações.");

            return ocorrencia;
        }

        private async Task AssociaSubOcorrenciasNaCriacao(Ocorrencia novaOcorrenciaPai, CreateOrEditOcorrenciaDTO dto)
        {
            if (dto.SubOcorrenciasId != null && dto.SubOcorrenciasId.Any())
            {
                var subOcorrencias = await _context.Ocorrencia
                    .Where(e => dto.SubOcorrenciasId.Contains(e.Id))
                    .ToListAsync();

                if (subOcorrencias.Count != dto.SubOcorrenciasId.Count)
                {
                    var idsEncontrados = subOcorrencias.Select(e => e.Id).ToList();
                    var idsNaoEncontrados = dto.SubOcorrenciasId.Except(idsEncontrados);
                    throw new InvalidOperationException($"Os seguintes IDs de sub-ocorrencias não foram encontrados: {string.Join(", ", idsNaoEncontrados)}");
                }

                foreach (var subOcorrencia in subOcorrencias)
                {
                    subOcorrencia.OcorrenciaPaiId = novaOcorrenciaPai.Id;
                }
            }
        }

        private async Task<List<Natureza>> ValidarEBuscarNaturezasAsync(List<string>? naturezasId)
        {
            if (naturezasId == null || !naturezasId.Any())
            {
                return new List<Natureza>();
            }

            // 1. Busca as naturezas solicitadas
            var naturezas = await _context.Naturezas
                                   .Where(n => naturezasId.Contains(n.Id))
                                   .ToListAsync();

            // 2. Valida se todos os IDs foram encontrados
            var uniqueRequestedIdsCount = naturezasId.Distinct().Count();
            if (naturezas.Count != uniqueRequestedIdsCount)
            {
                var foundIds = naturezas.Select(n => n.Id).ToHashSet();
                var missingIds = naturezasId.Distinct().Where(id => !foundIds.Contains(id));

                throw new ArgumentException($"As seguintes IDs de naturezas não foram encontradas: {string.Join(", ", missingIds)}");
            }

            var idsSelecionados = naturezas.Select(n => n.Id).ToList();

            var paisInvalidos = await _context.Naturezas
                .Where(n => n.NaturezaPaiId != null && idsSelecionados.Contains(n.NaturezaPaiId))
                .Select(n => n.NaturezaPaiId)
                .Distinct()
                .ToListAsync();

            if (paisInvalidos.Any())
            {
                var nomesInvalidos = naturezas
                    .Where(n => paisInvalidos.Contains(n.Id))
                    .Select(n => n.Nome);

                throw new ArgumentException($"As seguintes naturezas são categorias e não podem ser selecionadas: {string.Join(", ", nomesInvalidos)}");
            }

            return naturezas;
        }

        private async Task AtualizaRelacionamentoNaturezasAsync(Ocorrencia ocorrenciaParaAtualizar, List<Natureza> naturezasParaAssociar)
        {
            await _context.Entry(ocorrenciaParaAtualizar)
                .Collection(e => e.Naturezas)
                .LoadAsync();

            var naturezasAtuais = ocorrenciaParaAtualizar.Naturezas ?? new List<Natureza>();
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
        private async Task ValidarOcorrenciaPaiAsync(string? ocorrenciaPaiId)
        {
            if (string.IsNullOrWhiteSpace(ocorrenciaPaiId))
                return;
            var paiExiste = await _context.Ocorrencia.AnyAsync(e => e.Id == ocorrenciaPaiId);
            if (!paiExiste)
                throw new InvalidOperationException($"O ocorrencia pai com o ID '{ocorrenciaPaiId}' não foi encontrado.");
        }
    }
}
