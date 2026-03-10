using SIG_DefesaCivil.API.Data.DTO.Ocorrencia;
using SIG_DefesaCivil.API.Data.Enums;
using SIG_DefesaCivil.API.Data.Models.Shared;
using SIG_DefesaCivil.API.Helper;

namespace SIG_DefesaCivil.API.Mappers
{
    public static class CamposMapper
    {
        public static OcorrenciaCampos ToEntity(this OcorrenciaCamposDTO dto)
        {
            if (dto == null) return new OcorrenciaCampos();

            var campos = new OcorrenciaCampos
            {
                // 1. Mapeie os tipos primitivos e datas diretamente
                DataEHoraDoOcorrido = dto.DataEHoraDoOcorrido,
                DataEHoraInicioAtendimento = dto.DataEHoraInicioAtendimento,
                DataEHoraTerminoAtendimento = dto.DataEHoraTerminoAtendimento,
                Solicitante = dto.Solicitante,
                PossuiIPTU = dto.PossuiIPTU,
                NumeroDeMoradias = dto.NumeroDeMoradias,
                NumeroDeComodos = dto.NumeroDeComodos,
                NumeroDePavimentos = dto.NumeroDePavimentos,
                PossuiUnidadeFamiliar = dto.PossuiUnidadeFamiliar,
                NumeroDeDeficientes = dto.NumeroDeDeficientes,
                NumeroDeCriancas = dto.NumeroDeCriancas,
                NumeroDeAdultos = dto.NumeroDeAdultos,
                NumeroDeIdosos = dto.NumeroDeIdosos,

                // Mapeie o Endereço (Value Object)
                Localizacao = dto.Localizacao != null ? new Endereco
                {
                    Rua = dto.Localizacao.Rua,
                    Numero = dto.Localizacao.Numero,
                    Complemento = dto.Localizacao.Complemento,
                    Bairro = dto.Localizacao.Bairro,
                    CEP = dto.Localizacao.CEP,
                    Latitude = dto.Localizacao.Latitude,
                    Longitude = dto.Localizacao.Longitude
                } : new Endereco(),

                // 2. Mapeie as Listas de Enums usando nosso Helper
                AnalisePreliminar = dto.AnalisePreliminar.ToEnumList<EAnalisePreliminar>(),
                CaracterizacaoDoLocal = dto.CaracterizacaoDoLocal.ToEnumList<ECaracterizacaoLocal>(),
                Edificacao = dto.Edificacao.ToEnumList<ETipoEdificacao>(),
                Estrutura = dto.Estrutura.ToEnumList<ETipoEstrutura>(),
                TipoDeRisco = dto.TipoDeRisco.ToEnumList<ETipoRisco>(),
                TipificacaoDaOcorrencia = dto.TipificacaoDaOcorrencia.ToEnumList<ETipificacaoOcorrencia>(),
                Motivacao = dto.Motivacao.ToEnumList<EMotivacao>(),
                AreasAfetadas = dto.AreasAfetadas.ToEnumList<EAreaAfetada>()
            };

            // 3. Tratamento seguro para os Enums únicos
            if (Enum.TryParse<EGrauRisco>(dto.GrauDeRisco, out var grau))
                campos.GrauDeRisco = grau;

            if (Enum.TryParse<ERegimeOcupacao>(dto.RegimeDeOcupacaoDoImovel, out var regime))
                campos.RegimeDeOcupacaoDoImovel = regime;

            return campos;
        }

        public static OcorrenciaCamposDTO ToDto(this OcorrenciaCampos entity)
        {
            if (entity == null) return new OcorrenciaCamposDTO();

            var dto = new OcorrenciaCamposDTO
            {
                // Dados primitivos
                DataEHoraDoOcorrido = entity.DataEHoraDoOcorrido,
                DataEHoraInicioAtendimento = entity.DataEHoraInicioAtendimento,
                DataEHoraTerminoAtendimento = entity.DataEHoraTerminoAtendimento,
                Solicitante = entity.Solicitante,
                PossuiIPTU = entity.PossuiIPTU,
                NumeroDeMoradias = entity.NumeroDeMoradias,
                NumeroDeComodos = entity.NumeroDeComodos,
                NumeroDePavimentos = entity.NumeroDePavimentos,
                PossuiUnidadeFamiliar = entity.PossuiUnidadeFamiliar,
                NumeroDeDeficientes = entity.NumeroDeDeficientes,
                NumeroDeCriancas = entity.NumeroDeCriancas,
                NumeroDeAdultos = entity.NumeroDeAdultos,
                NumeroDeIdosos = entity.NumeroDeIdosos,

                // Tratamento seguro dos Enums únicos (Para string)
                GrauDeRisco = entity.GrauDeRisco?.ToString(),
                RegimeDeOcupacaoDoImovel = entity.RegimeDeOcupacaoDoImovel?.ToString(),

                // Listas de Enums (Usando o nosso Helper ToStringList)
                AnalisePreliminar = entity.AnalisePreliminar.ToStringList(),
                CaracterizacaoDoLocal = entity.CaracterizacaoDoLocal.ToStringList(),
                Edificacao = entity.Edificacao.ToStringList(),
                Estrutura = entity.Estrutura.ToStringList(),
                TipoDeRisco = entity.TipoDeRisco.ToStringList(),
                TipificacaoDaOcorrencia = entity.TipificacaoDaOcorrencia.ToStringList(),
                Motivacao = entity.Motivacao.ToStringList(),
                AreasAfetadas = entity.AreasAfetadas.ToStringList()
            };

            // Tratamento do Value Object Endereco -> DTO
            if (entity.Localizacao != null)
            {
                dto.Localizacao = new Endereco
                {
                    Rua = entity.Localizacao.Rua,
                    Numero = entity.Localizacao.Numero,
                    Complemento = entity.Localizacao.Complemento,
                    Bairro = entity.Localizacao.Bairro,
                    CEP = entity.Localizacao.CEP,
                    Latitude = entity.Localizacao.Latitude,
                    Longitude = entity.Localizacao.Longitude
                };
            }

            return dto;
        }
    }
}