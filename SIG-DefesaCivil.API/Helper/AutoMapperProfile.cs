using AutoMapper;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.DTO.Etapas;
using SIG_DefesaCivil.API.DTO.Eventos; // Ocorrencia DTOs estão aqui
using SIG_DefesaCivil.API.DTO.Quadros;
using SIG_DefesaCivil.API.DTOs; // NaturezaDTOs gerais
using SIG_DefesaCivil.API.Enums;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Ocorrencia; // Namespace da entidade Ocorrencia

namespace SIG_DefesaCivil.API.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ==========================================================
            // 1. AUXILIARES (Civil, Endereço, Usuário)
            // ==========================================================

            CreateMap<Civil, CivilDTO>();
            CreateMap<CivilDTO, Civil>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Id)));

            CreateMap<EnderecoDTO, Endereco>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Id)));
            CreateMap<Endereco, EnderecoDTO>(); // ReverseMap simples

            CreateMap<Usuario, DetalhesUsuarioDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.UserName)); // Ajuste se tiver campo 'Nome' real

            // ==========================================================
            // 2. OCORRÊNCIAS (ANTIGOS EVENTOS)
            // ==========================================================

            // --- Ocorrencia -> OcorrenciaPreviewDTO (Listagem/Kanban) ---
            CreateMap<Ocorrencia, OcorrenciaPreviewDTO>()
                .ForMember(dest => dest.EmailResponsavel, opt => opt.MapFrom(src => src.UsuarioCriador.NormalizedEmail))
                .ForMember(dest => dest.StageName, opt => opt.MapFrom(src => src.Etapa.Nome))
                .ForMember(dest => dest.SolicitanteNome, opt => opt.MapFrom(src => src.Solicitante.Nome))
                .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.Solicitante.CPF))
                // Formatação resumida do endereço para o card
                .ForMember(dest => dest.EnderecoResumido, opt => opt.MapFrom(src =>
                    src.Endereco != null ? $"{src.Endereco.Rua}, {src.Endereco.Numero} - {src.Endereco.Bairro}" : "Endereço não informado"))
                // Converte List<Enum> para List<String> para o card ver o risco
                .ForMember(dest => dest.TipoDeRisco, opt => opt.MapFrom(src => src.TipoDeRisco.Select(e => e.ToString()).ToList()));


            // --- CreateOrEditOcorrenciaDTO -> Ocorrencia (Entrada) ---
            CreateMap<CreateOrEditOcorrenciaDTO, Ocorrencia>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())

                // Campos ignorados pois são gerenciados pelo Service
                .ForMember(dest => dest.UsuarioCriadorId, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioCriador, opt => opt.Ignore())
                .ForMember(dest => dest.OcorrenciaPai, opt => opt.Ignore())
                .ForMember(dest => dest.OcorrenciaPaiId, opt => opt.Ignore())
                .ForMember(dest => dest.SubOcorrencias, opt => opt.Ignore()) // Corrigido nome
                .ForMember(dest => dest.Naturezas, opt => opt.Ignore())
                .ForMember(dest => dest.Anexos, opt => opt.Ignore())
                .ForMember(dest => dest.DataEntradaNaFaseAtual, opt => opt.Ignore())
                .ForMember(dest => dest.isVisible, opt => opt.Ignore())

                // -- Conversão de List<String> (DTO) para List<Enum> (Model) --
                // O AutoMapper geralmente faz isso se os nomes baterem, mas ser explícito é mais seguro
                // Se você tiver problemas, use .MapFrom(src => src.Prop.Select(s => Enum.Parse<TEnum>(s)).ToList())
                // Por enquanto, vamos confiar na conversão implícita do AutoMapper para Enums.
                ;


            // --- Ocorrencia -> OcorrenciaDetalhesDTO (Saída Completa) ---
            CreateMap<Ocorrencia, OcorrenciaDetalhesDTO>()
                .ForMember(dest => dest.UsuarioCriador, opt => opt.MapFrom(src => src.UsuarioCriador))
                .ForMember(dest => dest.SubEventos, opt => opt.MapFrom(src => src.SubOcorrencias)) // Mapeia para lista de Previews
                .ForMember(dest => dest.EventoPai, opt => opt.MapFrom(src => src.OcorrenciaPai)) // Mapeia para Preview
                .ForMember(dest => dest.Naturezas, opt => opt.MapFrom(src => src.Naturezas)) // Mapeia Natureza -> Resumo
                .ForMember(dest => dest.StageName, opt => opt.MapFrom(src => src.Etapa.Nome))
                .ForMember(dest => dest.Anexos, opt => opt.Ignore()) // Anexos carregados separadamente no service

                // Conversão de List<Enum> para List<String> para o DTO
                .ForMember(dest => dest.AnalisePreliminar, opt => opt.MapFrom(src => src.AnalisePreliminar.Select(e => e.ToString()).ToList()))
                .ForMember(dest => dest.CaracterizacaoDoLocal, opt => opt.MapFrom(src => src.CaracterizacaoDoLocal.Select(e => e.ToString()).ToList()))
                .ForMember(dest => dest.Edificacao, opt => opt.MapFrom(src => src.Edificacao.Select(e => e.ToString()).ToList()))
                .ForMember(dest => dest.Estrutura, opt => opt.MapFrom(src => src.Estrutura.Select(e => e.ToString()).ToList()))
                .ForMember(dest => dest.TipoDeRisco, opt => opt.MapFrom(src => src.TipoDeRisco.Select(e => e.ToString()).ToList()))
                .ForMember(dest => dest.TipificacaoDaOcorrencia, opt => opt.MapFrom(src => src.TipificacaoDaOcorrencia.Select(e => e.ToString()).ToList()))
                .ForMember(dest => dest.Motivacao, opt => opt.MapFrom(src => src.Motivacao.Select(e => e.ToString()).ToList()))
                .ForMember(dest => dest.AreasAfetadas, opt => opt.MapFrom(src => src.AreasAfetadas.Select(e => e.ToString()).ToList()))

                // Single Selects (Enum -> String)
                .ForMember(dest => dest.GrauDeRisco, opt => opt.MapFrom(src => src.GrauDeRisco.ToString()))
                .ForMember(dest => dest.RegimeDeOcupacaoDoImovel, opt => opt.MapFrom(src => src.RegimeDeOcupacaoDoImovel.ToString()));


            // ==========================================================
            // 3. OUTROS (Naturezas, Anexos, Quadros, Etapas)
            // ==========================================================

            // Naturezas
            CreateMap<Natureza, NaturezaResumoDTO>();
            CreateMap<Natureza, NaturezaDTO>();
            CreateMap<CreateNaturezaDTO, Natureza>();

            // Anexos
            CreateMap<Anexo, AnexoDTO>();

            // Quadros
            CreateMap<Quadro, QuadroDTO>();
            CreateMap<Quadro, QuadroDetalhesDTO>();
            CreateMap<CriarOuEditarQuadroDTO, Quadro>();

            // Etapas
            CreateMap<Etapa, EtapaDTO>()
                .ForMember(dest => dest.Ocorrencias, opt => opt.MapFrom(src => src.Ocorrencias)); // Ocorrencia -> Preview
            CreateMap<CriaOuAtualizaEtapaDTO, Etapa>();
        }
    }
}