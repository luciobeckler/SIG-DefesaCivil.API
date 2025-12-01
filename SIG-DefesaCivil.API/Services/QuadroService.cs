using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO.Quadros;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Services
{
    public class QuadroService
    {
        private readonly DefesaCivilDbContext _context;
        private readonly IMapper _mapper;

        public QuadroService(DefesaCivilDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<QuadroDTO>> ListarTodosAsync()
        {
            return await _context.Quadros
                .OrderBy(q => q.Nome)
                .AsNoTracking()
                .ProjectTo<QuadroDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<QuadroDetalhesDTO> ObterPorIdAsync(string id)
        {
            var quadro = await _context.Quadros
                .AsNoTracking()
                .AsSplitQuery()
                .Include(q => q.Etapas)
                    .ThenInclude(e => e.Eventos)
                        .ThenInclude(ev => ev.UsuarioCriador)
                .Include(q => q.Etapas)
                    .ThenInclude(e => e.Eventos)
                        .ThenInclude(ev => ev.Naturezas)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quadro == null)
                throw new KeyNotFoundException("Quadro não encontrado.");

            quadro.Etapas = quadro.Etapas
                .OrderBy(s => s.Posicao)
                .ToList();

            return _mapper.Map<QuadroDetalhesDTO>(quadro);
        }

        public async Task<QuadroDTO> CriarAsync(CriarOuEditarQuadroDTO dto)
        {
            var quadro = _mapper.Map<Quadro>(dto);
            quadro.Id = Guid.NewGuid().ToString();

            _context.Quadros.Add(quadro);
            await _context.SaveChangesAsync();

            return _mapper.Map<QuadroDTO>(quadro);
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