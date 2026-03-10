using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Mappers
{
    public static class QuadroMapper
    {
        public static QuadroDTO ToDto(this Quadro entity)
        {
            if (entity == null) return null!;

            return new QuadroDTO
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Descricao = entity.Descricao,
                Etapas = entity.Etapas
                    .Select(e => e.ToDto())
                    .ToList()
            };
        }

        public static Quadro ToEntity(this CriarOuEditarQuadroDTO dto)
        {
            if (dto == null) return null!;

            return new Quadro
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao
            };
        }
    }
}
