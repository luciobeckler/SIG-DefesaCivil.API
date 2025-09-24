using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTOs;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Services
{
    public class NaturezaService
    {
        private readonly DefesaCivilDbContext _context;

        public NaturezaService(DefesaCivilDbContext context)
        {
            _context = context;
        }

        // Lista todas as naturezas
        public async Task<List<NaturezaDto>> GetAllAsync()
        {
            var naturezas = await _context.Natureza
                .Include(n => n.SubNaturezas)
                .ToListAsync();

            return naturezas.Select(n => new NaturezaDto
            {
                Id = n.Id,
                Nome = n.Nome,
                CodigoNatureza = n.CodigoNatureza,
                NaturezaPaiId = n.NaturezaPaiId
            }).ToList();
        }

        // Busca uma natureza pelo ID
        public async Task<NaturezaDto?> GetByIdAsync(string id)
        {
            var natureza = await _context.Natureza
                .Include(n => n.SubNaturezas)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (natureza == null) return null;

            return new NaturezaDto
            {
                Id = natureza.Id,
                Nome = natureza.Nome,
                CodigoNatureza = natureza.CodigoNatureza,
                NaturezaPaiId = natureza.NaturezaPaiId
            };
        }

        // Adiciona nova natureza
        public async Task<NaturezaDto> CreateAsync(CreateNaturezaDto dto)
        {
            if (!string.IsNullOrEmpty(dto.NaturezaPaiId))
            {
                var pai = await _context.Natureza.FindAsync(dto.NaturezaPaiId);
                if (pai == null)
                    throw new ArgumentException("Natureza pai não encontrada");
            }

            var codigoExistente = await _context
                .Natureza.AnyAsync(n => n.CodigoNatureza == dto.CodigoNatureza);

            if (codigoExistente)
                throw new ArgumentException("Já existe uma natureza com este código.");

            var natureza = new Natureza
            {
                Id = Guid.NewGuid().ToString(),
                Nome = dto.Nome,
                CodigoNatureza = dto.CodigoNatureza,
                NaturezaPaiId = string.IsNullOrEmpty(dto.NaturezaPaiId) ? null : dto.NaturezaPaiId
            };

            _context.Natureza.Add(natureza);
            await _context.SaveChangesAsync();

            // Retorna DTO, não a entidade (evita loop)
            return new NaturezaDto
            {
                Id = natureza.Id,
                Nome = natureza.Nome,
                CodigoNatureza = natureza.CodigoNatureza,
                NaturezaPaiId = natureza.NaturezaPaiId
            };
        }

        // Atualiza natureza
        public async Task<bool> UpdateAsync(string id, CreateNaturezaDto dto)
        {
            var natureza = await _context.Natureza.FindAsync(id);
            if (natureza == null) return false;

            if (!string.IsNullOrEmpty(dto.NaturezaPaiId))
            {
                var pai = await _context.Natureza.FindAsync(dto.NaturezaPaiId);
                if (pai == null)
                    throw new ArgumentException("Natureza pai não encontrada");
            }

            var codigoExistente = await _context
                .Natureza.AnyAsync(n => n.CodigoNatureza == dto.CodigoNatureza);

            if (codigoExistente)
                throw new ArgumentException("Já existe uma natureza com este código.");

            natureza.Nome = dto.Nome;
            natureza.CodigoNatureza = dto.CodigoNatureza;
            natureza.NaturezaPaiId = dto.NaturezaPaiId;

            await _context.SaveChangesAsync();
            return true;
        }

        // Remove natureza
        public async Task<bool> DeleteAsync(string id)
        {
            var natureza = await _context.Natureza
                .Include(n => n.SubNaturezas)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (natureza == null) return false;

            async Task RemoverSubNaturezas(Natureza n)
            {
                if (n.SubNaturezas != null && n.SubNaturezas.Any())
                {
                    foreach (var sub in n.SubNaturezas.ToList())
                    {
                        var subCompleta = await _context.Natureza
                            .Include(x => x.SubNaturezas)
                            .FirstOrDefaultAsync(x => x.Id == sub.Id);

                        if (subCompleta != null)
                        {
                            await RemoverSubNaturezas(subCompleta);
                            _context.Natureza.Remove(subCompleta);
                        }
                    }
                }
            }

            await RemoverSubNaturezas(natureza);
            _context.Natureza.Remove(natureza);

            await _context.SaveChangesAsync();
            return true;
        }

        // Buscar irmãs (naturezas com mesmo pai)
        public async Task<List<NaturezaDto>> GetIrmasAsync(string id)
        {
            var natureza = await _context.Natureza.FindAsync(id);
            if (natureza == null) return new List<NaturezaDto>();

            var irmas = await _context.Natureza
                .Where(n => n.NaturezaPaiId == natureza.NaturezaPaiId && n.Id != id)
                .ToListAsync();

            return irmas.Select(n => new NaturezaDto
            {
                Id = n.Id,
                Nome = n.Nome,
                CodigoNatureza = n.CodigoNatureza,
                NaturezaPaiId = n.NaturezaPaiId
            }).ToList();
        }
    }
}
