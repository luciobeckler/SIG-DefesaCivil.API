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

        public async Task<List<NaturezaDto>> GetAllAsync()
        {
            var todasNaturezas = await _context.Natureza
                .Include(n => n.SubNaturezas)
                .ToListAsync();

            var raizes = todasNaturezas
                .Where(n => n.NaturezaPaiId == null)
                .OrderBy(n => n.CodigoNatureza)
                .ToList();

            return raizes.Select(n => MapToDto(n, todasNaturezas)).ToList();
        }

        public async Task<NaturezaDto?> GetByCodigoAsync(string codigo)
        {
            var natureza = await ObterNaturezaPorCodigo(codigo);
            return natureza is null ? null : MapToDto(natureza, new List<Natureza>());
        }

        public async Task<NaturezaDto> CreateAsync(CreateNaturezaDto dto)
        {
            await ValidarCodigoDuplicado(dto.CodigoNatureza);

            string? naturezaPaiId = null;

            if (!string.IsNullOrEmpty(dto.CodigoNaturezaPai))
            {
                var naturezaPai = await ObterNaturezaPorCodigo(dto.CodigoNaturezaPai);
                if (naturezaPai is null)
                    throw new ArgumentException("Natureza pai não encontrada");

                naturezaPaiId = naturezaPai.Id;
            }

            var natureza = new Natureza
            {
                Id = Guid.NewGuid().ToString(),
                Nome = dto.Nome,
                CodigoNatureza = dto.CodigoNatureza,
                NaturezaPaiId = naturezaPaiId
            };

            _context.Natureza.Add(natureza);
            await _context.SaveChangesAsync();

            return MapToDto(natureza, new List<Natureza>());
        }

        public async Task<bool> UpdateAsync(string id, CreateNaturezaDto dto)
        {
            var natureza = await ObterNaturezaPorId(id);
            if (natureza is null) return false;

            var codigoEmUso = await _context.Natureza
                .AnyAsync(n => n.CodigoNatureza == dto.CodigoNatureza && n.Id != id);

            if (codigoEmUso)
                throw new ArgumentException("Já existe uma natureza com este código.");

            if (!string.IsNullOrEmpty(dto.CodigoNaturezaPai))
            {
                var pai = await ObterNaturezaPorCodigo(dto.CodigoNaturezaPai);
                if (pai is null)
                    throw new ArgumentException("Natureza pai não encontrada");

                natureza.NaturezaPaiId = pai.Id;
            }
            else
            {
                natureza.NaturezaPaiId = null;
            }

            natureza.Nome = dto.Nome;
            natureza.CodigoNatureza = dto.CodigoNatureza;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string codigo)
        {
            var natureza = await _context.Natureza
                .Include(n => n.SubNaturezas)
                .FirstOrDefaultAsync(n => n.CodigoNatureza == codigo);

            if (natureza is null) return false;

            await RemoverSubNaturezasRecursivamente(natureza);
            _context.Natureza.Remove(natureza);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<NaturezaDto>> GetIrmasAsync(string codigo)
        {
            var natureza = await ObterNaturezaPorCodigo(codigo);
            if (natureza is null) return new List<NaturezaDto>();

            var irmas = await _context.Natureza
                .Where(n => n.NaturezaPaiId == natureza.NaturezaPaiId && n.Id != natureza.Id)
                .ToListAsync();

            return irmas.Select(n => MapToDto(n, new List<Natureza>())).ToList();
        }

        private NaturezaDto MapToDto(Natureza n, List<Natureza> todasNaturezas)
        {
            var dto = new NaturezaDto
            {
                Id = n.Id,
                Nome = n.Nome,
                CodigoNatureza = n.CodigoNatureza,
                NaturezaPaiId = n.NaturezaPaiId
            };

            var subNaturezas = todasNaturezas
                .Where(sn => sn.NaturezaPaiId == n.Id)
                .OrderBy(sn => sn.CodigoNatureza)
                .ToList();

            foreach (var sub in subNaturezas)
            {
                dto.SubNaturezas.Add(MapToDto(sub, todasNaturezas));
            }

            return dto;
        }

        private async Task ValidarCodigoDuplicado(string codigo)
        {
            var duplicado = await _context.Natureza
                .AnyAsync(n => n.CodigoNatureza == codigo);

            if (duplicado)
                throw new ArgumentException("Já existe uma natureza com este código.");
        }

        private async Task<Natureza?> ObterNaturezaPorId(string id) =>
            await _context.Natureza.FirstOrDefaultAsync(n => n.Id == id);

        private async Task<Natureza?> ObterNaturezaPorCodigo(string codigo) =>
            await _context.Natureza.FirstOrDefaultAsync(n => n.CodigoNatureza == codigo);

        private async Task RemoverSubNaturezasRecursivamente(Natureza n)
        {
            if (n.SubNaturezas is null || !n.SubNaturezas.Any()) return;

            foreach (var sub in n.SubNaturezas.ToList())
            {
                var subCompleta = await _context.Natureza
                    .Include(x => x.SubNaturezas)
                    .FirstOrDefaultAsync(x => x.CodigoNatureza == sub.CodigoNatureza);

                if (subCompleta is null) continue;

                await RemoverSubNaturezasRecursivamente(subCompleta);
                _context.Natureza.Remove(subCompleta);
            }
        }
    }
}
