using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Data.DTO.Ocorrencia;
using SIG_DefesaCivil.API.Data.Enums;
using SIG_DefesaCivil.API.Data.Models.Shared;
using SIG_DefesaCivil.API.Models.Ocorrencia;

namespace SIG_DefesaCivil.API.Mappers
{
    public static class OcorrenciaMapper
    {
        public static OcorrenciaCampos ToCamposEntity(this CreateOrEditOcorrenciaDTO dto)
        {
            if (dto == null) return null!;

            return dto.Campos.ToEntity();
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

        public static CreateOrEditOcorrenciaDTO ToCreateOcorrenciaDTO(
            this SolicitacaoVistoriaDTO vistoriaDto,
            ETipoCadastroOcorrencia tipoCadastro)
        {
            if (vistoriaDto == null)
                throw new ArgumentNullException(nameof(vistoriaDto));

            return new CreateOrEditOcorrenciaDTO
            {
                TipoCadastro = tipoCadastro,

                Campos = new OcorrenciaCamposDTO
                {
                    DataEHoraDoOcorrido = DateTime.Now,
                    Solicitante = vistoriaDto.Solicitante,
                    Localizacao = vistoriaDto.Localizacao
                }
            };
        }
    }
}