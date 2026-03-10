using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Mappers;

namespace SIG_DefesaCivil.API.Services
{
    public class QuadroService
    {
        private readonly DefesaCivilDbContext _context;

        public QuadroService(DefesaCivilDbContext context)
        {
            _context = context;
        }

        public async Task<List<QuadroDTO>> ListarTodosAsync()
        {
            var quadros = await _context.Quadros
                .AsNoTracking()
                .AsSplitQuery()
                .Include(q => q.Etapas)
                    .ThenInclude(e => e.Ocorrencias)
                        .ThenInclude(ev => ev.Responsavel)
                .Include(q => q.Etapas)
                    .ThenInclude(e => e.Ocorrencias)
                .ToListAsync();

            foreach (var quadro in quadros)
            {
                if (quadro.Etapas != null)
                {
                    quadro.Etapas = quadro.Etapas
                        .OrderBy(s => s.Posicao)
                        .ToList();
                }
            }

            return quadros
                .Select(q => q.ToDto())
                .ToList();
        }

        public async Task<QuadroDTO> ObterPorIdAsync(string id)
        {
            var quadro = await _context.Quadros
                .AsNoTracking()
                .AsSplitQuery()
                .Include(q => q.Etapas)
                    .ThenInclude(e => e.Ocorrencias)
                        .ThenInclude(ev => ev.Responsavel)
                .Include(q => q.Etapas)
                    .ThenInclude(e => e.Ocorrencias)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quadro == null)
                throw new KeyNotFoundException("Quadro não encontrado.");

            quadro.Etapas = quadro.Etapas
                .OrderBy(s => s.Posicao)
                .ToList();

            return quadro.ToDto();
        }

        public async Task<QuadroDTO> CriarAsync(CriarOuEditarQuadroDTO dto)
        {
            var quadro = dto.ToEntity();
            quadro.Id = Guid.NewGuid().ToString();

            _context.Quadros.Add(quadro);
            await _context.SaveChangesAsync();

            return quadro.ToDto();
        }

        public async Task AtualizarAsync(string id, CriarOuEditarQuadroDTO dto)
        {
            var quadro = await _context.Quadros.FindAsync(id);
            if (quadro == null)
                throw new KeyNotFoundException("Quadro não encontrado.");

            // Atualiza os campos
            quadro.Nome = dto.Nome;
            quadro.Descricao = dto.Descricao;

            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(string id)
        {
            var quadro = await _context.Quadros.FindAsync(id);
            if (quadro == null)
                throw new KeyNotFoundException("Quadro não encontrado.");

            // A exclusão em cascata do EF Core deve cuidar das Etapas e Eventos
            // Se configurado com OnDelete(DeleteBehavior.Cascade)
            _context.Quadros.Remove(quadro);
            await _context.SaveChangesAsync();
        }
    }
}