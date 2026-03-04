using AutoMapper;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Data.DTO.Ocorrencia;
using SIG_DefesaCivil.API.Data.Enums;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Ocorrencia;

namespace SIG_DefesaCivil.API.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = false;

            CreateMap<Usuario, DetalhesUsuarioDTO>()
                 .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.UserName));

            CreateMap<Ocorrencia, OcorrenciaPreviewDTO>()
                .ForMember(dest => dest.EmailResponsavel, opt => opt.MapFrom(src => src.UsuarioCriador.NormalizedEmail))
                .ForMember(dest => dest.EnderecoResumido, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Campos.Localizacao.Rua)
                    ? "Endereço não informado"
                    : $"{src.Campos.Localizacao.Rua}, {src.Campos.Localizacao.Numero}"))
                .ForMember(dest => dest.TipoDeRisco, opt => opt.MapFrom(src => ToStringList(src.Campos.TipoDeRisco)));

            CreateMap<CreateOrEditOcorrenciaDTO, Ocorrencia>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioCriadorId, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioCriador, opt => opt.Ignore())
                .ForMember(dest => dest.OcorrenciaPai, opt => opt.Ignore())
                .ForMember(dest => dest.OcorrenciaPaiId, opt => opt.Ignore())
                .ForMember(dest => dest.SubOcorrencias, opt => opt.Ignore())
                .ForMember(dest => dest.Naturezas, opt => opt.Ignore())
                .ForMember(dest => dest.Anexos, opt => opt.Ignore())
                .ForMember(dest => dest.DataEntradaNaFaseAtual, opt => opt.Ignore())
                .ForMember(dest => dest.isVisivel, opt => opt.Ignore())

                // Usando ForPath para propriedades aninhadas
                .ForPath(dest => dest.Campos.AnalisePreliminar, opt => opt.MapFrom(src => ToEnumList<EAnalisePreliminar>(src.Campos.AnalisePreliminar)))
                .ForPath(dest => dest.Campos.CaracterizacaoDoLocal, opt => opt.MapFrom(src => ToEnumList<ECaracterizacaoLocal>(src.Campos.CaracterizacaoDoLocal)))
                .ForPath(dest => dest.Campos.Edificacao, opt => opt.MapFrom(src => ToEnumList<ETipoEdificacao>(src.Campos.Edificacao)))
                .ForPath(dest => dest.Campos.Estrutura, opt => opt.MapFrom(src => ToEnumList<ETipoEstrutura>(src.Campos.Estrutura)))
                .ForPath(dest => dest.Campos.TipoDeRisco, opt => opt.MapFrom(src => ToEnumList<ETipoRisco>(src.Campos.TipoDeRisco)))
                .ForPath(dest => dest.Campos.TipificacaoDaOcorrencia, opt => opt.MapFrom(src => ToEnumList<ETipificacaoOcorrencia>(src.Campos.TipificacaoDaOcorrencia)))
                .ForPath(dest => dest.Campos.Motivacao, opt => opt.MapFrom(src => ToEnumList<EMotivacao>(src.Campos.Motivacao)))
                .ForPath(dest => dest.Campos.AreasAfetadas, opt => opt.MapFrom(src => ToEnumList<EAreaAfetada>(src.Campos.AreasAfetadas)));


            // --- Ocorrencia -> OcorrenciaDetalhesDTO (Saída Completa) ---
            CreateMap<Ocorrencia, OcorrenciaOffilineDTO>()
                .ForMember(dest => dest.Anexos, opt => opt.Ignore())

                // Usando ForPath para propriedades aninhadas
                .ForPath(dest => dest.Campos.AnalisePreliminar, opt => opt.MapFrom(src => ToStringList(src.Campos.AnalisePreliminar)))
                .ForPath(dest => dest.Campos.CaracterizacaoDoLocal, opt => opt.MapFrom(src => ToStringList(src.Campos.CaracterizacaoDoLocal)))
                .ForPath(dest => dest.Campos.Edificacao, opt => opt.MapFrom(src => ToStringList(src.Campos.Edificacao)))
                .ForPath(dest => dest.Campos.Estrutura, opt => opt.MapFrom(src => ToStringList(src.Campos.Estrutura)))
                .ForPath(dest => dest.Campos.TipoDeRisco, opt => opt.MapFrom(src => ToStringList(src.Campos.TipoDeRisco)))
                .ForPath(dest => dest.Campos.TipificacaoDaOcorrencia, opt => opt.MapFrom(src => ToStringList(src.Campos.TipificacaoDaOcorrencia)))
                .ForPath(dest => dest.Campos.Motivacao, opt => opt.MapFrom(src => ToStringList(src.Campos.Motivacao)))
                .ForPath(dest => dest.Campos.AreasAfetadas, opt => opt.MapFrom(src => ToStringList(src.Campos.AreasAfetadas)))

                // Single Selects com ForPath
                .ForPath(dest => dest.Campos.GrauDeRisco, opt => opt.MapFrom(src => src.Campos.GrauDeRisco.ToString()))
                .ForPath(dest => dest.Campos.RegimeDeOcupacaoDoImovel, opt => opt.MapFrom(src => src.Campos.RegimeDeOcupacaoDoImovel.ToString()));

            CreateMap<Natureza, NaturezaResumoDTO>();
            CreateMap<Natureza, NaturezaDTO>();
            CreateMap<CreateNaturezaDTO, Natureza>();

            CreateMap<Anexo, AnexoDTO>();

            CreateMap<Quadro, QuadroDTO>();
            CreateMap<Quadro, QuadroDetalhesDTO>();
            CreateMap<CriarOuEditarQuadroDTO, Quadro>();

            CreateMap<Etapa, EtapaDTO>();
            CreateMap<CriaOuAtualizaEtapaDTO, Etapa>();
        }

        private static List<TEnum> ToEnumList<TEnum>(IEnumerable<string>? source) where TEnum : struct, Enum
        {
            if (source == null || !source.Any()) return new List<TEnum>();
            return source.Select(s => Enum.Parse<TEnum>(s)).ToList();
        }

        private static List<string> ToStringList<TEnum>(IEnumerable<TEnum>? source) where TEnum : struct, Enum
        {
            if (source == null || !source.Any()) return new List<string>();
            return source.Select(e => e.ToString()).ToList();
        }
    }
}