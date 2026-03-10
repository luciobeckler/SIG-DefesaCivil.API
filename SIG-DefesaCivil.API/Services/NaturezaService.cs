using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Mappers;
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

        public async Task<List<NaturezaDTO>> GetAllAsync()
        {
            var todasNaturezas = await _context.Naturezas
                .Include(n => n.SubNaturezas)
                .ToListAsync();

            var raizes = todasNaturezas
                .Where(n => n.NaturezaPaiId == null)
                .OrderBy(n => n.CodigoNatureza)
                .ToList();

            return raizes.Select(raiz => raiz.ToDto()).ToList();
        }

        public async Task<NaturezaDTO> GetByCodigoAsync(string codigo)
        {
            var natureza = await ObterNaturezaPorCodigo(codigo);
            if (natureza is null)
                throw new ArgumentException("Natureza não encontrada");

            return natureza.ToDto();
        }

        public async Task<NaturezaDTO> GetByIdAsync(string id)
        {
            var natureza = await _context.Naturezas.FirstOrDefaultAsync(n => n.Id == id);
            if (natureza is null)
                throw new ArgumentException("Natureza não encontrada");

            return natureza.ToDto();
        }

        public async Task<NaturezaDTO> CreateAsync(CreateNaturezaDTO dto)
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

            var natureza = dto.ToEntity();
            natureza.Id = Guid.NewGuid().ToString();
            natureza.NaturezaPaiId = naturezaPaiId;

            _context.Naturezas.Add(natureza);
            await _context.SaveChangesAsync();

            return natureza.ToDto();
        }

        public async Task<bool> UpdateAsync(string id, CreateNaturezaDTO dto)
        {
            var natureza = await ObterNaturezaPorId(id);
            if (natureza is null) return false;

            var codigoEmUso = await _context.Naturezas
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

        public async Task<bool> DeleteAsync(string id)
        {
            var natureza = await _context.Naturezas
                .Include(n => n.SubNaturezas)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (natureza is null) return false;

            await RemoverSubNaturezasRecursivamente(natureza);
            _context.Naturezas.Remove(natureza);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<NaturezaDTO>> GetIrmasAsync(string codigo)
        {
            var natureza = await ObterNaturezaPorCodigo(codigo);
            if (natureza is null)
                return new List<NaturezaDTO>();

            var irmas = await _context.Naturezas
                .Where(n => n.NaturezaPaiId == natureza.NaturezaPaiId && n.Id != natureza.Id)
                .ToListAsync();

            return irmas.Select(n => n.ToDto()).ToList();
        }

        private async Task ValidarCodigoDuplicado(string codigo)
        {
            var duplicado = await _context.Naturezas
                .AnyAsync(n => n.CodigoNatureza == codigo);

            if (duplicado)
                throw new ArgumentException("Já existe uma natureza com este código.");
        }

        private async Task<Natureza?> ObterNaturezaPorId(string id) =>
            await _context.Naturezas.FirstOrDefaultAsync(n => n.Id == id);

        private async Task<Natureza?> ObterNaturezaPorCodigo(string codigo) =>
            await _context.Naturezas.FirstOrDefaultAsync(n => n.CodigoNatureza == codigo);

        private async Task RemoverSubNaturezasRecursivamente(Natureza n)
        {
            if (n.SubNaturezas is null || !n.SubNaturezas.Any()) return;

            foreach (var sub in n.SubNaturezas.ToList())
            {
                var subCompleta = await _context.Naturezas
                    .Include(x => x.SubNaturezas)
                    .FirstOrDefaultAsync(x => x.Id == sub.Id);

                if (subCompleta is null) continue;

                await RemoverSubNaturezasRecursivamente(subCompleta);
                _context.Naturezas.Remove(subCompleta);
            }
        }
    }
}
