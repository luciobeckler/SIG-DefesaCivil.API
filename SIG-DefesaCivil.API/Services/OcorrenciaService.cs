using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Data.DTO.Ocorrencia;
using SIG_DefesaCivil.API.Data.Enums;
using SIG_DefesaCivil.API.Data.Models.Ocorrencias;
using SIG_DefesaCivil.API.Mappers;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Ocorrencia;

namespace SIG_DefesaCivil.API.Services
{
    public class OcorrenciaService
    {
        private readonly DefesaCivilDbContext _context;
        private readonly EtapaService _etapaService;
        private readonly UserManager<Usuario> _userManager;

        public OcorrenciaService(
            DefesaCivilDbContext context,
            EtapaService etapaService,
            UserManager<Usuario> userManager
            )
        {
            _context = context;
            _etapaService = etapaService;
            _userManager = userManager;
        }



        public async Task<OcorrenciaDTO> OcorrenciaDetalheById(string id, Usuario usuario)
        {
            var ocorrencia = await RecuperaOcorrenciaCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(ocorrencia.ResponsavelId, usuario);

            string acao = "Visualizou detalhes";
            AdicionaOuAtualizaHistorico(ocorrencia.Id, usuario.Id, acao);

            var anexos = await _context.Anexos
                .Where(a => a.EntidadeId == ocorrencia.Id && a.TipoEntidade == ETiposEntidades.Ocorrencia)
                .AsNoTracking()
                .ToListAsync();

            var ocorrenciaDto = ocorrencia.ToDto();

            ocorrenciaDto.Anexos = anexos.Select(a => a.ToDto()).ToList();

            await _context.SaveChangesAsync();
            return ocorrenciaDto;
        }

        public async Task<Ocorrencia> CriarAsync(string quadroId, CreateOrEditOcorrenciaDTO dto, Usuario? usuario)
        {
            var anoProtocolo = DateTime.UtcNow.Year;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string novoNumeroProtocolo;

                // Busca a última ocorrência APENAS daquele ano específico
                // Ordenamos por DataCriacao (ou Id) decrescente para pegar o último inserido
                var ultimaOcorrenciaDoAno = await _context.Ocorrencia
                    .Where(x => x.Protocolo.StartsWith($"{anoProtocolo}-"))
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => x.Protocolo)
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
                var ocorrencia = dto.ToEntity();

                ocorrencia.Id = Guid.NewGuid().ToString();
                ocorrencia.Protocolo = novoNumeroProtocolo; // Atribui o número gerado
                ocorrencia.ResponsavelId = usuario == null ? null : usuario.Id;
                ocorrencia.isVisivel = true;

                _context.Ocorrencia.Add(ocorrencia);

                // Adiciona à etapa
                await _etapaService.AdicionaOcorrenciaNaPrimeiraEtapaAsync(ocorrencia, quadroId);

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
            var ocorrencia = await RecuperaOcorrenciaCompletoPorId(id);
            VerificaSeUsuarioPossuiPermissao(ocorrencia.ResponsavelId, usuario);

            ocorrencia = dto.ToEntity();
            ocorrencia.Id = id;

            var acao = "Editou ocorrencia";
            AdicionaOuAtualizaHistorico(ocorrencia.Id, usuario.Id, acao);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(string id, Usuario usuario)
        {
            var ocorrencia = await _context.Ocorrencia
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ocorrencia == null)
            {
                throw new InvalidOperationException($"O ocorrencia com o ID '{id}' não foi encontrado.");
            }

            bool podeDeletar = usuario.Cargo == nameof(ECargos.Administrador) || usuario.Cargo == nameof(ECargos.Diretor);
            if (!podeDeletar)
                throw new UnauthorizedAccessException("Você não tem permissão para excluir ocorrencias.");

            // Soft Delete
            ocorrencia.isVisivel = false;
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
                .Where(a => a.EntidadeId == ocorrenciaId && a.TipoEntidade == ETiposEntidades.Ocorrencia)
                .AsNoTracking()
                .ToListAsync();

            return anexos.Select(a => a.ToDto()).ToList();
        }

        public async Task TransicionaOcorrencia(Usuario usuario, string ocorrenciaId, string etapaAtualId, string etapaDestinoId)
        {
            var ocorrencia = await _context.Ocorrencia
                .FirstOrDefaultAsync(o => o.Id == ocorrenciaId);

            ArgumentNullException.ThrowIfNull(ocorrencia, "Ocorrencia não encontrada");

            var etapaAtual = await _etapaService.GetEtapaById(etapaAtualId);
            var etapaDestino = await _etapaService.GetEtapaById(etapaDestinoId);

            _etapaService.VerificaRegrasDeTransicao(usuario, ocorrencia, etapaAtual, etapaDestino);

            var transicao = new Transicao
            {
                DataEHorario = DateTime.Now,
                Ocorrencia = ocorrencia,
                Responsavel = usuario,
                EtapaAtual = etapaDestino,
                EtapaAnterior = etapaAtual,
            };

            ocorrencia.Transicoes.Add(transicao);
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
                        Console.WriteLine($"   -> Ocorrência {ocorrencia.Protocolo} movida para próxima etapa.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   -> [ERRO] Falha ao mover {ocorrencia.Protocolo}: {ex.Message}");
                    }
                }
            }
        }
        public async Task<List<Transicao>> GetTransicoesByOcorrenciaId(string ocorrenciaId)
        {
            var transicoes = await _context.Set<Transicao>()
                .Where(t => t.OcorrenciaId == ocorrenciaId)
                .ToListAsync();

            return transicoes;
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
                .Include(o => o.Responsavel)
                .Include(o => o.Transicoes.OrderBy(t => t.DataEHorario))
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ocorrencia == null)
                throw new InvalidOperationException($"O ocorrencia com o ID '{id}' não foi encontrado.");

            if (!ocorrencia.isVisivel)
                throw new InvalidOperationException($"O ocorrencia com o ID '{id}' foi deletado, entre em contato com o administrador para mais informações.");

            return ocorrencia;
        }
    }
}
