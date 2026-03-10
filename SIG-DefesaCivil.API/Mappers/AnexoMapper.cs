using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Mappers
{
    public static class AnexoMapper
    {
        public static AnexoDTO ToDto(this Anexo entity)
        {
            if (entity == null) return null!;

            return new AnexoDTO
            {
                Id = entity.Id,
                NomeOriginal = entity.NomeOriginal,
                TamanhoBytes = entity.TamanhoBytes,
                TipoConteudo = entity.TipoConteudo,
                UrlArmazenamento = entity.UrlArmazenamento,
                Localizacao = entity.Localizacao,
                DataHoraCaptura = entity.DataHoraCaptura,
            };
        }
    }
}
