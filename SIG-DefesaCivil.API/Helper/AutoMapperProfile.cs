// SIG_DefesaCivil.API/Helper/AutoMapperProfile.cs
using AutoMapper;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.DTO.Etapas;
using SIG_DefesaCivil.API.DTO.Eventos;
using SIG_DefesaCivil.API.DTO.Quadros;
using SIG_DefesaCivil.API.DTOs; // Assume que NaturezaDTO e CreateNaturezaDTO estão aqui
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ==== Eventos ====

            CreateMap<Evento, EventoPreviewDTO>()
                .ForMember(
                dest => dest.EmailResponsavel, opt => opt.MapFrom(src => src.UsuarioCriador.NormalizedEmail));

            CreateMap<CreateOrEditEventoDTO, Evento>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioCriadorId, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioCriador, opt => opt.Ignore())
                .ForMember(dest => dest.EventoPai, opt => opt.Ignore())
                .ForMember(dest => dest.EventoPaiId, opt => opt.Ignore())
                .ForMember(dest => dest.SubEventos, opt => opt.Ignore())
                .ForMember(dest => dest.Naturezas, opt => opt.Ignore());

            CreateMap<Evento, EventoDetalhesDTO>()
                .ForMember(dest => dest.UsuarioCriador, opt => opt.MapFrom(src => src.UsuarioCriador))
                .ForMember(dest => dest.SubEventos, opt => opt.MapFrom(src => src.SubEventos))
                .ForMember(dest => dest.EventoPai, opt => opt.MapFrom(src => src.EventoPai))
                .ForMember(dest => dest.Naturezas, opt => opt.MapFrom(src => src.Naturezas))
                .ForMember(dest => dest.Anexos, opt => opt.Ignore());

            // ==== Usuários (Para EventoDetalhesDTO) ====
            CreateMap<Usuario, DetalhesUsuarioDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.UserName)); 

            // ==== Naturezas ====
            CreateMap<Natureza, NaturezaResumoDTO>(); 
            CreateMap<Natureza, NaturezaDTO>();
            CreateMap<CreateNaturezaDTO, Natureza>();

            // ==== Anexos ====
            CreateMap<Anexo, AnexoDTO>();

            // --- Quadros ---
            CreateMap<Quadro, QuadroDTO>();
            CreateMap<Quadro, QuadroDetalhesDTO>();
            CreateMap<CriarOuEditarQuadroDTO, Quadro>();

            // --- Etapas ---
            CreateMap<Etapa, EtapaDTO>()
                .ForMember(dest => dest.Eventos, opt => opt.MapFrom(src => src.Eventos));
            CreateMap<CriaOuAtualizaEtapaDTO, Etapa>();
        }
    }
}