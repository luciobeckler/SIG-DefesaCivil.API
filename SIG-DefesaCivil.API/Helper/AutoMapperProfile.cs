using AutoMapper;
using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Data.DTO.Ocorrencia;
using SIG_DefesaCivil.API.Data.Enums;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Ocorrencia; // Namespace da entidade Ocorrencia

namespace SIG_DefesaCivil.API.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ==========================================================
            // 1. AUXILIARES
            // ==========================================================

            CreateMap<Usuario, DetalhesUsuarioDTO>()
                 .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.UserName)); // Ajuste se tiver campo 'Nome' real

            // ==========================================================
            // 2. OCORRÊNCIAS
            // ==========================================================

            // --- Ocorrencia -> OcorrenciaPreviewDTO (Listagem/Kanban) ---
            CreateMap<Ocorrencia, OcorrenciaPreviewDTO>()
                .ForMember(dest => dest.EmailResponsavel, opt => opt.MapFrom(src => src.UsuarioCriador.NormalizedEmail))
                // Montagem do Endereço Resumido
                .ForMember(dest => dest.EnderecoResumido, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Campos.Localizacao.Rua) ? "Endereço não informado" : $"{src.Campos.Localizacao.Rua}, {src.Campos.Localizacao.Rua}"))
                // Conversão Segura de Lista de Enums -> String (com verificação de nulo)
                .ForMember(dest => dest.TipoDeRisco, opt => opt.MapFrom(src =>
                    src.Campos.TipoDeRisco != null ? src.Campos.TipoDeRisco.Select(e => e.ToString()).ToList() : new List<string>()));


            // --- CreateOrEditOcorrenciaDTO -> Ocorrencia (Entrada) ---
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
                .ForMember(dest => dest.isVisible, opt => opt.Ignore())

                // --- Conversão Explícita de List<String> para List<Enum> ---
                .ForMember(dest => dest.Campos.AnalisePreliminar, opt => opt.MapFrom(src => src.AnalisePreliminar.Select(s => Enum.Parse<EAnalisePreliminar>(s)).ToList()))
                .ForMember(dest => dest.Campos.CaracterizacaoDoLocal, opt => opt.MapFrom(src => src.CaracterizacaoDoLocal.Select(s => Enum.Parse<ECaracterizacaoLocal>(s)).ToList()))
                .ForMember(dest => dest.Campos.Edificacao, opt => opt.MapFrom(src => src.Edificacao.Select(s => Enum.Parse<ETipoEdificacao>(s)).ToList()))
                .ForMember(dest => dest.Campos.Estrutura, opt => opt.MapFrom(src => src.Estrutura.Select(s => Enum.Parse<ETipoEstrutura>(s)).ToList()))
                .ForMember(dest => dest.Campos.TipoDeRisco, opt => opt.MapFrom(src => src.TipoDeRisco.Select(s => Enum.Parse<ETipoRisco>(s)).ToList()))
                .ForMember(dest => dest.Campos.TipificacaoDaOcorrencia, opt => opt.MapFrom(src => src.TipificacaoDaOcorrencia.Select(s => Enum.Parse<ETipificacaoOcorrencia>(s)).ToList()))
                .ForMember(dest => dest.Campos.Motivacao, opt => opt.MapFrom(src => src.Motivacao.Select(s => Enum.Parse<EMotivacao>(s)).ToList()))
                .ForMember(dest => dest.Campos.AreasAfetadas, opt => opt.MapFrom(src => src.AreasAfetadas.Select(s => Enum.Parse<EAreaAfetada>(s)).ToList()));


            // --- Ocorrencia -> OcorrenciaDetalhesDTO (Saída Completa) ---
            CreateMap<Ocorrencia, OcorrenciaDetalhesDTO>()
                .ForMember(dest => dest.UsuarioCriador, opt => opt.MapFrom(src => src.UsuarioCriador))
                .ForMember(dest => dest.SubOcorrencias, opt => opt.MapFrom(src => src.SubOcorrencias))
                .ForMember(dest => dest.OcorrenciaPai, opt => opt.MapFrom(src => src.OcorrenciaPai))
                .ForMember(dest => dest.Naturezas, opt => opt.MapFrom(src => src.Naturezas))
                .ForMember(dest => dest.Anexos, opt => opt.Ignore())

                // Conversão de List<Enum> para List<String> (Com Null Check)
                .ForMember(dest => dest.AnalisePreliminar, opt => opt.MapFrom(src => src.Campos.AnalisePreliminar != null ? src.Campos.AnalisePreliminar.Select(e => e.ToString()).ToList() : new List<string>()))
                .ForMember(dest => dest.CaracterizacaoDoLocal, opt => opt.MapFrom(src => src.Campos.CaracterizacaoDoLocal != null ? src.Campos.CaracterizacaoDoLocal.Select(e => e.ToString()).ToList() : new List<string>()))
                .ForMember(dest => dest.Edificacao, opt => opt.MapFrom(src => src.Campos.Edificacao != null ? src.Campos.Edificacao.Select(e => e.ToString()).ToList() : new List<string>()))
                .ForMember(dest => dest.Estrutura, opt => opt.MapFrom(src => src.Campos.Estrutura != null ? src.Campos.Estrutura.Select(e => e.ToString()).ToList() : new List<string>()))
                .ForMember(dest => dest.TipoDeRisco, opt => opt.MapFrom(src => src.Campos.TipoDeRisco != null ? src.Campos.TipoDeRisco.Select(e => e.ToString()).ToList() : new List<string>()))
                .ForMember(dest => dest.TipificacaoDaOcorrencia, opt => opt.MapFrom(src => src.Campos.TipificacaoDaOcorrencia != null ? src.Campos.TipificacaoDaOcorrencia.Select(e => e.ToString()).ToList() : new List<string>()))
                .ForMember(dest => dest.Motivacao, opt => opt.MapFrom(src => src.Campos.Motivacao != null ? src.Campos.Motivacao.Select(e => e.ToString()).ToList() : new List<string>()))
                .ForMember(dest => dest.AreasAfetadas, opt => opt.MapFrom(src => src.Campos.AreasAfetadas != null ? src.Campos.AreasAfetadas.Select(e => e.ToString()).ToList() : new List<string>()))

                // Single Selects (Enum -> String)
                .ForMember(dest => dest.GrauDeRisco, opt => opt.MapFrom(src => src.Campos.GrauDeRisco.HasValue ? src.Campos.GrauDeRisco.ToString() : null))
                .ForMember(dest => dest.RegimeDeOcupacaoDoImovel, opt => opt.MapFrom(src => src.Campos.RegimeDeOcupacaoDoImovel.HasValue ? src.Campos.RegimeDeOcupacaoDoImovel.ToString() : null));

            // ==========================================================
            // 3. OUTROS
            // ==========================================================
            CreateMap<Natureza, NaturezaResumoDTO>();
            CreateMap<Natureza, NaturezaDTO>();
            CreateMap<CreateNaturezaDTO, Natureza>();

            CreateMap<Anexo, AnexoDTO>();

            CreateMap<Quadro, QuadroDTO>();
            CreateMap<Quadro, QuadroDetalhesDTO>();
            CreateMap<CriarOuEditarQuadroDTO, Quadro>();

            CreateMap<Etapa, EtapaDTO>()
                .ForMember(dest => dest.Ocorrencias, opt => opt.MapFrom(src => src.Ocorrencias)); // Ocorrencia -> Preview
            CreateMap<CriaOuAtualizaEtapaDTO, Etapa>();
        }
    }
}