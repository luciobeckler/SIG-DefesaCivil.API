using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.DTO.Eventos;
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

        public OcorrenciaService(
            DefesaCivilDbContext context, 
            IMapper mapper, 
            AnexoService anexoService,
            EtapaService etapaService
            )
        {
            _context = context;
            _mapper = mapper;
            _anexoService = anexoService;
            _etapaService = etapaService;
        }

        public async Task<Ocorrencia> GetOcorrenciaPreviewById(string id)
        {
            var ocorrencia = await _context.Ocorrencia
                .FirstOrDefaultAsync(e => e.Id.Equals(id));

            if (ocorrencia == null)
            {
                throw new KeyNotFoundException("Evento não encontrado");
            }

            return ocorrencia;
        }

        public async Task<OcorrenciaDetalhesDTO> DetalhesEventosPorId(string id, Usuario usuario)
        {
            var ocorrencia = await RecuperaEventoCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(ocorrencia.UsuarioCriadorId, usuario);

            string acao = "Visualizou detalhes";
            AdicionaOuAtualizaHistorico(ocorrencia.Id, usuario.Id, acao);

            // Busca os anexos genéricos associados a este ocorrencia
            var anexos = await _context.Anexos
                .Where(a => a.EntidadeId == ocorrencia.Id && a.TipoEntidade == "Evento")
                .AsNoTracking()
                .ToListAsync();

            // Mapeia o ocorrencia principal
            var ocorrenciaDto = _mapper.Map<OcorrenciaDetalhesDTO>(ocorrencia);

            // Mapeia e atribui os anexos
            ocorrenciaDto.Anexos = _mapper.Map<ICollection<AnexoDTO>>(anexos);

            await _context.SaveChangesAsync(); // Salva o histórico
            return ocorrenciaDto;
        }

        public async Task<Ocorrencia> CriarAsync(Usuario usuario, string etapaId, CreateOrEditOcorrenciaDTO dto)
        {
            // --- 1. Validações ---
            await ValidarCodigoUnicoAsync(dto.Numero);
            ValidarHierarquiaUnica(dto);
            await ValidarEventoPaiAsync(dto.EventoPaiId);
            var naturezas = await ValidarEBuscarNaturezasAsync(dto.NaturezasId);

            // --- 2. Mapeamento e Criação ---
            var ocorrencia = _mapper.Map<Ocorrencia>(dto); // Mapeia campos simples

            // Define propriedades gerenciadas manualmente
            ocorrencia.Id = Guid.NewGuid().ToString();
            ocorrencia.UsuarioCriadorId = usuario.Id;
            ocorrencia.OcorrenciaPaiId = string.IsNullOrWhiteSpace(dto.EventoPaiId) ? null : dto.EventoPaiId;
            ocorrencia.Naturezas = naturezas;
            ocorrencia.isVisible = true; // Valor padrão definido na entidade

            await AssociaSubEventosNaCriacao(ocorrencia, dto);

            // --- 3. Salva o Evento Principal ---
            _context.Ocorrencia.Add(ocorrencia);

            await _etapaService.AdicionaEventoNaPrimeiraEtapaAsync(usuario, ocorrencia, etapaId);
            await _context.SaveChangesAsync();

            return ocorrencia;
        }

        public async Task AtualizarAsync(string id, CreateOrEditOcorrenciaDTO dto, Usuario usuario)
        {
            // --- 1. Validações ---
            await ValidarCodigoUnicoAsync(dto.Numero, id);
            ValidarHierarquiaUnica(dto);
            var naturezasParaAssociar = await ValidarEBuscarNaturezasAsync(dto.NaturezasId);

            // --- 2. Busca da Entidade ---
            var ocorrencia = await RecuperaEventoCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(ocorrencia.UsuarioCriadorId, usuario);

            if (!string.IsNullOrWhiteSpace(dto.EventoPaiId) && dto.EventoPaiId == id)
            {
                throw new InvalidOperationException("Um ocorrencia não pode ser definido como seu próprio ocorrencia pai.");
            }

            // --- 3. Mapeamento e Atualização de Relações ---
            _mapper.Map(dto, ocorrencia); // Atualiza campos simples (Titulo, Descricao, etc.)

            await AtualizaEventoPaiAsync(ocorrencia, dto);
            await AtualizaRelacionamentoSubEventosAsync(ocorrencia, dto);

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

            return await _context.OcorrenciasHistoricos
                .Where(h => h.OcorrenciaId == ocorrenciaId)
                .OrderByDescending(h => h.UltimaAlteracao)
                .ToListAsync();
        }
        public async Task<ICollection<AnexoDTO>> GetAnexosDTOByEventoIdAsync(string ocorrenciaId)
        {
            var anexos = await _context.Anexos
                .Where(a => a.EntidadeId == ocorrenciaId && a.TipoEntidade == "Evento")
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<ICollection<AnexoDTO>>(anexos);
        }

        private async Task AtualizaEventoPaiAsync(Ocorrencia ocorrenciaParaAtualizar, CreateOrEditOcorrenciaDTO dto)
        {
            await ValidarEventoPaiAsync(dto.EventoPaiId);
            ocorrenciaParaAtualizar.OcorrenciaPaiId = string.IsNullOrWhiteSpace(dto.EventoPaiId) ? null : dto.EventoPaiId;
        }

        private async Task AtualizaRelacionamentoSubEventosAsync(Ocorrencia ocorrenciaParaAtualizar, CreateOrEditOcorrenciaDTO dto)
        {
            var novosIds = dto.SubEventosId?.ToHashSet() ?? new HashSet<string>();

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

            var subEventosParaRemover = ocorrenciaParaAtualizar.SubOcorrencias
                .Where(s => !novosIds.Contains(s.Id))
                .ToList();

            foreach (var subEvento in subEventosParaRemover)
            {
                ocorrenciaParaAtualizar.SubOcorrencias.Remove(subEvento);
                subEvento.OcorrenciaPaiId = null;
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

        private async Task ValidarCodigoUnicoAsync(string codigo, string? ocorrenciaIgnoradoId = null)
        {
            var query = _context.Ocorrencia.AsNoTracking().Where(e => e.Numero.ToUpper() == codigo.ToUpper());

            if (ocorrenciaIgnoradoId != null)
            {
                query = query.Where(e => e.Id != ocorrenciaIgnoradoId);
            }

            if (await query.AnyAsync())
            {
                throw new InvalidOperationException($"O código '{codigo}' já está em uso por outro ocorrencia.");
            }
        }

        private void AdicionaOuAtualizaHistorico(string ocorrenciaId, string usuarioId, string acao)
        {
            var registroUsuarioNoHistoricoEvento = _context.OcorrenciasHistoricos.FirstOrDefault(e =>
                e.OcorrenciaId == ocorrenciaId &&
                e.UsuarioId == usuarioId &&
                e.Acao == acao);

            if (registroUsuarioNoHistoricoEvento == null)
            {
                _context.OcorrenciasHistoricos.Add(new OcorrenciaHistorico
                {
                    OcorrenciaId = ocorrenciaId,
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
        private async Task<Ocorrencia> RecuperaEventoCompletoPorId(string id)
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

        private void ValidarHierarquiaUnica(CreateOrEditOcorrenciaDTO dto)
        {
            var idDoPai = dto.EventoPaiId;
            var idsDosSubEventos = dto.SubEventosId?.ToHashSet() ?? new HashSet<string>();

            if (string.IsNullOrWhiteSpace(idDoPai) || idsDosSubEventos.Count == 0)
            {
                return;
            }

            if (idsDosSubEventos.Contains(idDoPai))
            {
                throw new InvalidOperationException($"O ocorrencia pai (ID: {idDoPai}) não pode ser listado simultaneamente como um sub-ocorrencia.");
            }
        }

        private async Task AssociaSubEventosNaCriacao(Ocorrencia novoEventoPai, CreateOrEditOcorrenciaDTO dto)
        {
            if (dto.SubEventosId != null && dto.SubEventosId.Any())
            {
                var subEventos = await _context.Ocorrencia
                    .Where(e => dto.SubEventosId.Contains(e.Id))
                    .ToListAsync();

                if (subEventos.Count != dto.SubEventosId.Count)
                {
                    var idsEncontrados = subEventos.Select(e => e.Id).ToList();
                    var idsNaoEncontrados = dto.SubEventosId.Except(idsEncontrados);
                    throw new InvalidOperationException($"Os seguintes IDs de sub-ocorrencias não foram encontrados: {string.Join(", ", idsNaoEncontrados)}");
                }

                foreach (var subEvento in subEventos)
                {
                    subEvento.OcorrenciaPaiId = novoEventoPai.Id;
                }
            }
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

        private async Task AtualizaRelacionamentoNaturezasAsync(Ocorrencia ocorrenciaParaAtualizar, ICollection<Natureza> naturezasParaAssociar)
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
        private async Task ValidarEventoPaiAsync(string? ocorrenciaPaiId)
        {
            if (string.IsNullOrWhiteSpace(ocorrenciaPaiId)) 
                return;
            var paiExiste = await _context.Ocorrencia.AnyAsync(e => e.Id == ocorrenciaPaiId);
            if (!paiExiste) 
                throw new InvalidOperationException($"O ocorrencia pai com o ID '{ocorrenciaPaiId}' não foi encontrado.");
        }
    }
}
