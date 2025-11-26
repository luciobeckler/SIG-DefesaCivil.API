using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.DTO.Etapas;
using SIG_DefesaCivil.API.Models;

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
                    etapa.Posicao = i + 1; // Posição 1, 2, 3...
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

        public async Task TransicionaEvento(string eventoId, string etapaAtualId, string etapaDestinoId)
        {
            verificaSeEventoExiste(eventoId);
            verificaSeEtapaExiste(etapaAtualId);

            verificaRegrasDeTransicao(etapaAtualId, etapaDestinoId);
        }

        private async void verificaRegrasDeTransicao(string etapaAtualId, string etapaDestinoId)
        {

        }
    }
}