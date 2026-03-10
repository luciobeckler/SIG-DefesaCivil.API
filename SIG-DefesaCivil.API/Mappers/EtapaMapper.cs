using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Data.Enums;
using SIG_DefesaCivil.API.Helper;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Mappers
{
    public static class EtapaMapper
    {
        public static EtapaDTO ToDto(this Etapa entity)
        {
            if (entity == null) return null!;

            return new EtapaDTO
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Descricao = entity.Descricao,
                Posicao = entity.Posicao,
                QuadroId = entity.QuadroId,
                MinTempoNaEtapa = entity.MinTempoNaEtapa,
                MaxTempoNaEtapa = entity.MaxTempoNaEtapa,
                EtapasDestinoId = entity.EtapasDestinoId,
                PermissoesParaTransicionarParaEstaEtapa = entity
                        .PermissoesParaTransicionarParaEstaEtapa
                        .Select(cargo => cargo.ToString())
                        .ToList(),
                Ocorrencias = entity
                        .Ocorrencias
                        .Select(ocorrencia => ocorrencia.ToDto())
                        .ToList()
            };
        }

        public static Etapa ToEntity(this CriaOuAtualizaEtapaDTO dto)
        {
            if (dto == null) return null!;

            return new Etapa
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Posicao = dto.Posicao,
                QuadroId = dto.QuadroId,
                MinTempoNaEtapa = dto.MinTempoNaEtapa,
                MaxTempoNaEtapa = dto.MaxTempoNaEtapa,
                EtapasDestinoId = dto.EtapasDestinoId,
                PermissoesParaTransicionarParaEstaEtapa = dto
                        .PermissoesParaTransicionarParaEstaEtapa
                        .ToEnumList<ECargos>()
            };
        }
    }
}
