using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Services
{
    public class StageService
    {
        private readonly DefesaCivilDbContext _context;
        private readonly IMapper _mapper;

        public StageService(DefesaCivilDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Lista todos os Stages (colunas) de um Frame (quadro) específico, 
        /// ordenados por sua posição.
        /// </summary>
        public async Task<List<StageDTO>> GetStagesByFrameIdAsync(string frameId)
        {
            // Valida se o Frame existe
            if (!await _context.Frames.AnyAsync(f => f.Id == frameId))
            {
                throw new KeyNotFoundException("Quadro (Frame) não encontrado.");
            }

            return await _context.Stages
                .Where(s => s.FrameId == frameId)
                .OrderBy(s => s.Position)
                .AsNoTracking()
                // Projeta para o DTO, incluindo os eventos (cartões) dentro de cada stage
                .ProjectTo<StageDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        /// <summary>
        /// Busca um Stage (coluna) específico pelo seu ID.
        /// </summary>
        public async Task<StageDTO> GetByIdAsync(string id)
        {
            var stage = await _context.Stages
                .AsNoTracking()
                .Include(s => s.Eventos) // Inclui os eventos
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stage == null)
            {
                throw new KeyNotFoundException("Coluna (Stage) não encontrada.");
            }

            return _mapper.Map<StageDTO>(stage);
        }

        /// <summary>
        /// Cria um novo Stage (coluna) e o adiciona ao final de um Frame (quadro).
        /// </summary>
        public async Task<StageDTO> CreateAsync(CreateOrEditStageDTO dto)
        {
            // Valida se o Frame existe
            if (!await _context.Frames.AnyAsync(f => f.Id == dto.FrameId))
            {
                throw new KeyNotFoundException("Quadro (Frame) não encontrado.");
            }

            // Valida se o FormularioId (opcional) existe
            if (!string.IsNullOrEmpty(dto.FormularioId))
            {
                if (!await _context.Forms.AnyAsync(f => f.Id == dto.FormularioId))
                {
                    throw new ArgumentException("Formulário (FormularioId) inválido.");
                }
            }

            // Mapeia o DTO para a entidade
            var stage = _mapper.Map<Stage>(dto);
            stage.Id = Guid.NewGuid().ToString();

            // Calcula a posição (coloca no final do quadro)
            var maxPosition = await _context.Stages
                .Where(s => s.FrameId == dto.FrameId)
                .MaxAsync(s => (float?)s.Position) ?? 0f;

            stage.Position = maxPosition + 1;

            _context.Stages.Add(stage);
            await _context.SaveChangesAsync();

            return _mapper.Map<StageDTO>(stage);
        }

        /// <summary>
        /// Atualiza as propriedades de um Stage (ex: Nome, Descrição, FormularioId).
        /// </summary>
        public async Task UpdateAsync(string id, CreateOrEditStageDTO dto)
        {
            var stage = await _context.Stages.FindAsync(id);
            if (stage == null)
            {
                throw new KeyNotFoundException("Coluna (Stage) não encontrada.");
            }

            // Valida se o novo FormularioId (opcional) existe
            if (!string.IsNullOrEmpty(dto.FormularioId))
            {
                if (!await _context.Forms.AnyAsync(f => f.Id == dto.FormularioId))
                {
                    throw new ArgumentException("Formulário (FormularioId) inválido.");
                }
            }

            // Mapeia as propriedades atualizadas do DTO para a entidade
            _mapper.Map(dto, stage);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Atualiza a ordem (posição) dos Stages dentro de um Frame.
        /// </summary>
        public async Task UpdatePositionsAsync(string frameId, ReorderStagesDTO dto)
        {
            // Busca todos os stages do frame no banco
            var stagesDoFrame = await _context.Stages
                .Where(s => s.FrameId == frameId)
                .ToListAsync();

            // Atualiza a posição de cada stage baseado na ordem da lista recebida
            for (int i = 0; i < dto.StageIdsInOrder.Count; i++)
            {
                var stageId = dto.StageIdsInOrder[i];
                var stage = stagesDoFrame.FirstOrDefault(s => s.Id == stageId);

                if (stage != null)
                {
                    // Atribui a nova posição (base 1)
                    stage.Position = i + 1;
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Exclui um Stage (coluna).
        /// Os Eventos (cartões) dentro dele terão seu StageId definido como null.
        /// </summary>
        public async Task DeleteAsync(string id)
        {
            var stage = await _context.Stages
                .Include(s => s.Eventos) // Carrega os eventos associados
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stage == null)
            {
                throw new KeyNotFoundException("Coluna (Stage) não encontrada.");
            }

            // Desassocia os Eventos (cartões) deste Stage (coluna)
            // Isso evita que os eventos sejam excluídos em cascata.
            foreach (var evento in stage.Eventos)
            {
                evento.StageId = null;
            }

            // Agora, remove o Stage (coluna)
            _context.Stages.Remove(stage);
            await _context.SaveChangesAsync();
        }
    }
}