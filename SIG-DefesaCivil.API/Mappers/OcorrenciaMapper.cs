using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Data.DTO.Ocorrencia;
using SIG_DefesaCivil.API.Data.Enums;
using SIG_DefesaCivil.API.Models.Ocorrencia;
using System.Text.Json;

namespace SIG_DefesaCivil.API.Mappers
{
    public static class OcorrenciaMapper
    {
        // --- Entrada de Dados (Create/Update) ---
        public static Ocorrencia ToEntity(this CreateOrEditOcorrenciaDTO dto)
        {
            if (dto == null) return null!;

            return new Ocorrencia
            {
                Campos = dto.Campos.ToEntity(),
            };
        }

        public static OcorrenciaDTO ToDto(this Ocorrencia entity)
        {
            if (entity == null) return null!;

            return new OcorrenciaDTO
            {
                Id = entity.Id,
                Protocolo = entity.Protocolo,
                isVisible = entity.isVisivel,
                DataEntradaNaFaseAtual = entity.DataEntradaNaFaseAtual,
                Anexos = entity.Anexos.Select(a => a.ToDto()).ToList(),
                Responsavel = entity.Responsavel.ToDto(),
                Campos = entity.Campos.ToDto(),
            };
        }

        public static CreateOrEditOcorrenciaDTO ToCreateOcorrenciaDTO(this SolicitacaoVistoriaDTO dto, ETipoCadastroOcorrencia tipoCadastro)
        {
            if (dto == null) return null!;
            var ocorrenciaDto = JsonSerializer.Deserialize<CreateOrEditOcorrenciaDTO>(dto.DadosJson);
            ocorrenciaDto.TipoCadastro = tipoCadastro;

            return ocorrenciaDto;
        }
    }
}