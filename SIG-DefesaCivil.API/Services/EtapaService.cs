using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO.Etapas;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.Services
{
    public class EtapaService
    {
        private readonly DefesaCivilDbContext _context;
        private readonly IMapper _mapper;

        public EtapaService(DefesaCivilDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<EtapaDTO> CriarAsync(CriaOuAtualizaEtapaDTO dto)
        {
            // Verifica se o quadro existe
            var quadroExiste = await _context.Quadros.AnyAsync(q => q.Id == dto.QuadroId);
            if (!quadroExiste)
                throw new KeyNotFoundException("O Quadro informado não existe.");

            // Calcula a próxima posição (última + 1)
            var ultimaPosicao = await _context.Etapas
                .Where(e => e.QuadroId == dto.QuadroId)
                .MaxAsync(e => (int?)e.Posicao) ?? 0;

            var etapa = _mapper.Map<Etapa>(dto);
            etapa.Id = Guid.NewGuid().ToString();
            etapa.Posicao = ultimaPosicao + 1;

            _context.Etapas.Add(etapa);
            await _context.SaveChangesAsync();

            return _mapper.Map<EtapaDTO>(etapa);
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
                .Include(e => e.Eventos)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (etapa == null)
                throw new KeyNotFoundException("Etapa não encontrada.");

            // Opcional: Impedir exclusão se houver eventos (proteção de dados)
            if (etapa.Eventos != null && etapa.Eventos.Any())
            {
                throw new InvalidOperationException("Não é possível excluir uma etapa que contém eventos. Mova os eventos primeiro.");
            }

            _context.Etapas.Remove(etapa);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionaEventoNaPrimeiraEtapaAsync(Usuario usuario, Evento evento, string etapaId)
        {
            var etapa = await GetEtapaById(etapaId);
            if(etapa.Posicao != 0)
                throw new InvalidOperationException("Só é possíve criar o evento na primeira etapa do quadro.");

            VerificaPermissaoParaMudarParaFase(usuario, etapa);

            etapa.Eventos.Add(evento);
        }
        
        public async Task TransicionaEvento(Usuario usuario , Evento evento, string etapaAtualId, string etapaDestinoId)
        {
            var etapaAtual = await GetEtapaById(etapaAtualId);
            var etapaDestino = await GetEtapaById(etapaDestinoId);

            VerificaRegrasDeTransicao(usuario,evento,etapaAtual, etapaDestino);

            etapaAtual.Eventos.Remove(evento);
            etapaDestino.Eventos.Add(evento);
            evento.DataEntradaNaFaseAtual = DateTime.Now;

            await _context.SaveChangesAsync();
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

        private void VerificaRegrasDeTransicao(Usuario usuario, Evento evento, Etapa etapaAtual, Etapa etapaDestino)
        {
            VerificaPermissaoParaMudarParaFase(usuario, etapaDestino);
            VerificaEstadiaMinimaNaFase(evento, etapaAtual);
            VerificaPossibilidadeDeTransicaoParaFase(evento, etapaAtual, etapaDestino);
        }

        private void VerificaPermissaoParaMudarParaFase(Usuario usuario, Etapa etapaDestino)
        {
            if (!etapaDestino.PermissoesParaTransicionarParaEstaEtapa.Contains(usuario.Cargo))
            {
                throw new UnauthorizedAccessException("Você não tem permissão para transicionar o evento para a fase desejada.");
            }
        }

        private void VerificaEstadiaMinimaNaFase(Evento evento, Etapa etapaAtual)
        {
            if (DateTime.Now - evento.DataEntradaNaFaseAtual < etapaAtual.MinTempoNaEtapa)
            {
                throw new InvalidOperationException("Evento não permaneceu o tempo mínimo necessário na fase atual.");
            }
        }

        private void VerificaPossibilidadeDeTransicaoParaFase(Evento evento, Etapa etapaAtual, Etapa etapaDestino)
        {
            if (etapaAtual.EtapasDestinoId != null && !etapaAtual.EtapasDestinoId.Contains(etapaDestino.Id))
            {
                throw new InvalidOperationException($"Não é possível transicionar da etapa {etapaAtual.Nome} para {etapaDestino.Nome}.");
            }
        }
    }
}