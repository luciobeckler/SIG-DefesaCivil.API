using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Data.Enums;
using SIG_DefesaCivil.API.Mappers;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Ocorrencia;

namespace SIG_DefesaCivil.API.Services
{
    public class EtapaService
    {
        private readonly DefesaCivilDbContext _context;

        public EtapaService(DefesaCivilDbContext context)
        {
            _context = context;
        }

        public async Task<EtapaDTO> CriarAsync(CriaOuAtualizaEtapaDTO dto)
        {
            // Verifica se o quadro existe
            var quadroExiste = await _context.Quadros.AnyAsync(q => q.Id == dto.QuadroId);
            if (!quadroExiste)
                throw new KeyNotFoundException("O Quadro informado não existe.");

            var posicaoPreenchida = _context.Etapas
                .Where(e => e.Quadro.Id == dto.QuadroId && e.Posicao == dto.Posicao);
            if (posicaoPreenchida != null)
                throw new ArgumentException("Posição já se encontra preenchida");

            var etapa = dto.ToEntity();
            etapa.Id = Guid.NewGuid().ToString();
            etapa.Posicao = dto.Posicao;

            _context.Etapas.Add(etapa);
            await _context.SaveChangesAsync();

            return etapa.ToDto();
        }

        public async Task AtualizarAsync(string id, CriaOuAtualizaEtapaDTO dto)
        {
            var etapa = await _context.Etapas.FindAsync(id);
            if (etapa == null)
                throw new KeyNotFoundException("Etapa não encontrada.");

            // Atualiza apenas nome e descrição (não muda posição ou quadro aqui)
            etapa.Nome = dto.Nome;
            etapa.Descricao = dto.Descricao;

            await _context.SaveChangesAsync();
        }

        public async Task ReordenarEtapasAsync(string quadroId, List<string> idsDasEtapasNaOrdem)
        {
            var etapasDoQuadro = await _context.Etapas
                .Where(e => e.QuadroId == quadroId)
                .ToListAsync();

            // Atualiza a posição baseada no índice da lista recebida
            for (int i = 0; i < idsDasEtapasNaOrdem.Count; i++)
            {
                var idEtapa = idsDasEtapasNaOrdem[i];
                var etapa = etapasDoQuadro.FirstOrDefault(e => e.Id == idEtapa);

                if (etapa != null)
                {
                    etapa.Posicao = i; // Posição 0, 1, 2, 3...
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(string id)
        {
            var etapa = await _context.Etapas
                .Include(e => e.Ocorrencias)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (etapa == null)
                throw new KeyNotFoundException("Etapa não encontrada.");

            // Opcional: Impedir exclusão se houver ocorrencias (proteção de dados)
            if (etapa.Ocorrencias != null && etapa.Ocorrencias.Any())
            {
                throw new InvalidOperationException("Não é possível excluir uma etapa que contém ocorrencias. Mova os ocorrencias primeiro.");
            }

            _context.Etapas.Remove(etapa);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionaOcorrenciaNaPrimeiraEtapaAsync(Ocorrencia ocorrencia, string quadroId)
        {
            var primeiraEtapa = await _context.Etapas
                .Where(e => e.QuadroId == quadroId)
                .OrderBy(e => e.Posicao)
                .FirstOrDefaultAsync();

            if (primeiraEtapa == null)
            {
                throw new InvalidOperationException("Este quadro não possui etapas cadastradas.");
            }


            primeiraEtapa.Ocorrencias.Add(ocorrencia);
        }

        public async Task<Etapa> GetEtapaById(string id)
        {
            var etapa = await _context.Etapas
                .FirstOrDefaultAsync(e => e.Id.Equals(id));

            if (etapa == null)
            {
                throw new KeyNotFoundException("Etapa não encontrada");
            }

            return etapa;
        }

        public void VerificaRegrasDeTransicao(Usuario usuario, Ocorrencia ocorrencia, Etapa etapaAtual, Etapa etapaDestino)
        {
            //VerificaPermissaoParaMudarParaFase(usuario, etapaDestino);
            VerificaEstadiaMinimaNaFase(ocorrencia, etapaAtual);
            VerificaPossibilidadeDeTransicaoParaFase(ocorrencia, etapaAtual, etapaDestino);
        }

        private void VerificaPermissaoParaMudarParaFase(Usuario usuario, Etapa etapaDestino)
        {
            var listaDePermissoesVazia = etapaDestino.PermissoesParaTransicionarParaEstaEtapa.Count == 0;
            var usuarioPossuiPermissao = etapaDestino
                .PermissoesParaTransicionarParaEstaEtapa
                .Contains(Enum.Parse<ECargos>(usuario.Cargo));
            if (!listaDePermissoesVazia && !usuarioPossuiPermissao)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para transicionar o ocorrencia para a fase desejada.");
            }
        }

        private void VerificaEstadiaMinimaNaFase(Ocorrencia ocorrencia, Etapa etapaAtual)
        {
            if (DateTime.Now - ocorrencia.DataEntradaNaFaseAtual < etapaAtual.MinTempoNaEtapa)
            {
                throw new InvalidOperationException("Evento não permaneceu o tempo mínimo necessário na fase atual.");
            }
        }

        private void VerificaPossibilidadeDeTransicaoParaFase(Ocorrencia ocorrencia, Etapa etapaAtual, Etapa etapaDestino)
        {
            if (etapaAtual.EtapasDestinoId != null && !etapaAtual.EtapasDestinoId.Contains(etapaDestino.Id))
            {
                throw new InvalidOperationException($"Não é possível transicionar da etapa {etapaAtual.Nome} para {etapaDestino.Nome}.");
            }
        }
    }
}