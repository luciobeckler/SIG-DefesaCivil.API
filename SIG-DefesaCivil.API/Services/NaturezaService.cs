using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTOs;
using SIG_DefesaCivil.API.Helper;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Services
{
    public class NaturezaService
    {
        private readonly DefesaCivilDbContext _context;
        private readonly IMapper _mapper;

        public NaturezaService(DefesaCivilDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<NaturezaDTO>> GetAllAsync()
        {
            var todasNaturezas = await _context.Natureza
                .Include(n => n.SubNaturezas)
                .ToListAsync();

            var raizes = todasNaturezas
                .Where(n => n.NaturezaPaiId == null)
                .OrderBy(n => n.CodigoNatureza)
                .ToList();

            return _mapper.Map<List<NaturezaDTO>>(raizes);
        }

        public async Task<NaturezaDTO> GetByCodigoAsync(string codigo)
        {
            var natureza = await ObterNaturezaPorCodigo(codigo);
            if (natureza is null)
                throw new ArgumentException("Natureza não encontrada");
            
            return _mapper.Map<NaturezaDTO>(natureza);
        }

        public async Task<NaturezaDTO> GetByIdAsync(string id)
        {
            var natureza = await _context.Natureza.FirstOrDefaultAsync(n => n.Id == id);
            if (natureza is null)
                throw new ArgumentException("Natureza não encontrada");

            return _mapper.Map<NaturezaDTO>(natureza);
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

            var natureza = _mapper.Map<Natureza>(dto);
            natureza.Id = Guid.NewGuid().ToString();
            natureza.NaturezaPaiId = naturezaPaiId;

            _context.Natureza.Add(natureza);
            await _context.SaveChangesAsync();

            return _mapper.Map<NaturezaDTO>(natureza);
        }

        public async Task<bool> UpdateAsync(string id, CreateNaturezaDTO dto)
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

        public async Task<List<NaturezaDTO>> GetIrmasAsync(string codigo)
        {
            var natureza = await ObterNaturezaPorCodigo(codigo);
            if (natureza is null) 
                return new List<NaturezaDTO>();

            var irmas = await _context.Natureza
                .Where(n => n.NaturezaPaiId == natureza.NaturezaPaiId && n.Id != natureza.Id)
                .ToListAsync();

            return _mapper.Map<List<NaturezaDTO>>(irmas);
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
