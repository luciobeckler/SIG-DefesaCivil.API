using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Mappers
{
    public static class NaturezaMapper
    {
        public static Natureza ToEntity(this CreateNaturezaDTO dto)
        {
            if (dto == null) return null!;

            return new Natureza
            {
                Nome = dto.Nome,
                CodigoNatureza = dto.CodigoNatureza,
                Descricao = dto.Descricao,
                NaturezaPaiId = dto.CodigoNaturezaPai,
            };
        }

        public static NaturezaDTO ToDto(this Natureza dto)
        {
            if (dto == null) return null!;

            return new NaturezaDTO
            {
                Id = dto.Id,
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                CodigoNatureza = dto.CodigoNatureza,
                NaturezaPaiId = dto.NaturezaPaiId,
            };
        }
    }
}
