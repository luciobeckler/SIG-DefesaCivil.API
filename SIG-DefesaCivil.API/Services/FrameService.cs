using AutoMapper;
using AutoMapper.QueryableExtensions; // Para .ProjectTo<T>
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTOs.Frames;
using SIG_DefesaCivil.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIG_DefesaCivil.API.Services
{
    public class FrameService
    {
        private readonly DefesaCivilDbContext _context;
        private readonly IMapper _mapper;

        public FrameService(DefesaCivilDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Lista todos os Frames (Quadros) de forma leve, sem seus Stages.
        /// </summary>
        public async Task<List<FrameDTO>> GetAllAsync()
        {
            // Usa .ProjectTo para uma consulta SQL eficiente que só busca
            // os campos necessários para o FrameDTO.
            return await _context.Frames
                .OrderBy(f => f.Name)
                .AsNoTracking()
                .ProjectTo<FrameDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        /// <summary>
        /// Busca um Frame (Quadro) específico e todos os seus Stages e Eventos (Cartões)
        /// de forma hierárquica e ordenada.
        /// </summary>
        public async Task<FrameDetalhesDTO> GetByIdAsync(string id)
        {
            // Busca o Frame e faz Eager Loading (Include) dos Stages
            // e também dos Eventos dentro de cada Stage.
            var frame = await _context.Frames
                .AsNoTracking()
                .Include(f => f.Stages.OrderBy(s => s.Position)) // Ordena as colunas
                    .ThenInclude(s => s.Eventos.Where(e => e.isVisible)) // Carrega os "cartões"
                        .ThenInclude(e => e.UsuarioCriador) // Necessário para o EmailResponsavel
                .Include(f => f.Stages)
                    .ThenInclude(s => s.Eventos)
                        .ThenInclude(e => e.Naturezas) // Necessário para as tags de natureza
                .FirstOrDefaultAsync(f => f.Id == id);

            if (frame == null)
            {
                // Use uma exceção específica para o Controller tratar como 404
                throw new KeyNotFoundException("Quadro (Frame) não encontrado.");
            }

            // Mapeia a entidade completa para o DTO de Detalhes
            return _mapper.Map<FrameDetalhesDTO>(frame);
        }

        /// <summary>
        /// Cria um novo Frame (Quadro).
        /// </summary>
        public async Task<FrameDTO> CreateAsync(CreateOrEditFrameDTO dto)
        {
            var frame = _mapper.Map<Frame>(dto);
            frame.Id = Guid.NewGuid().ToString(); // Garante um novo ID

            _context.Frames.Add(frame);
            await _context.SaveChangesAsync();

            // Retorna o DTO simples do quadro recém-criado
            return _mapper.Map<FrameDTO>(frame);
        }

        /// <summary>
        /// Atualiza as propriedades de um Frame (ex: Nome, Descrição).
        /// O gerenciamento de Stages (colunas) é feito pelo StageService.
        /// </summary>
        public async Task UpdateAsync(string id, CreateOrEditFrameDTO dto)
        {
            // Busca a *entidade* do banco para rastreamento
            var frameEntity = await _context.Frames.FirstOrDefaultAsync(f => f.Id == id);

            if (frameEntity == null)
            {
                throw new KeyNotFoundException("Quadro (Frame) não encontrado.");
            }

            // Mapeia as propriedades do DTO para a entidade existente
            _mapper.Map(dto, frameEntity);

            await _context.SaveChangesAsync();
            // Não precisa retornar nada, o status 204 No Content no Controller é suficiente
        }

        /// <summary>
        /// Exclui um Frame. A configuração do DbContext (OnDelete Cascade)
        /// deve cuidar da exclusão dos Stages associados.
        /// </summary>
        public async Task DeleteAsync(string id)
        {
            var frame = await _context.Frames.FirstOrDefaultAsync(f => f.Id == id);

            if (frame == null)
            {
                throw new KeyNotFoundException("Quadro (Frame) não encontrado.");
            }

            _context.Frames.Remove(frame);
            await _context.SaveChangesAsync();
        }
    }
}