// SIG_DefesaCivil.API/Helper/AutoMapperProfile.cs
using AutoMapper;
using SIG_DefesaCivil.API.DTO;
using SIG_DefesaCivil.API.DTO.Eventos.SIG_DefesaCivil.API.DTO.EventoDTO; // Ajuste este namespace se necessário
using SIG_DefesaCivil.API.DTOs; // Assume que NaturezaDTO e CreateNaturezaDTO estão aqui
using SIG_DefesaCivil.API.DTOs.Frames;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ==== Eventos ====
            // DTO de Entrada -> Entidade
            CreateMap<CreateEventoDTO, Evento>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Stage, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioCriador, opt => opt.Ignore())
                // ... (ignora outras coleções como Naturezas, SubEventos)
                .ForMember(dest => dest.Status, opt => opt.Ignore()); // Ignora o Status antigo

            CreateMap<UpdateEventoDTO, Evento>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StageId, opt => opt.Ignore()) // Ignora StageId (movimentação é separada)
                .ForMember(dest => dest.Stage, opt => opt.Ignore())
                // ... (ignora outras coleções e navegações)
                .ForMember(dest => dest.Status, opt => opt.Ignore()); // Ignora o Status antigo

            // Entidade -> DTO de Preview
            CreateMap<Evento, EventoPreviewDTO>()
                .ForMember(dest => dest.EmailResponsavel, opt => opt.MapFrom(src => src.UsuarioCriador.Email))
                .ForMember(dest => dest.StageId, opt => opt.MapFrom(src => src.StageId));

            // Entidade -> DTO de Detalhes
            CreateMap<Evento, EventoDetalhesDTO>()
                .ForMember(dest => dest.UsuarioCriador, opt => opt.MapFrom(src => src.UsuarioCriador))
                .ForMember(dest => dest.SubEventos, opt => opt.MapFrom(src => src.SubEventos))
                .ForMember(dest => dest.EventoPai, opt => opt.MapFrom(src => src.EventoPai))
                .ForMember(dest => dest.Naturezas, opt => opt.MapFrom(src => src.Naturezas))
                .ForMember(dest => dest.StageId, opt => opt.MapFrom(src => src.StageId))
                .ForMember(dest => dest.Anexos, opt => opt.Ignore()); // Carregado separadamente

            // --- Outros Mapeamentos (Anexo, Natureza, Usuário) ---
            CreateMap<Anexo, AnexoDTO>();
            CreateMap<Natureza, NaturezaResumoDTO>();
            CreateMap<Usuario, EventoDetalhesUsuarioDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.UserName));

            // ==== Usuários (Para EventoDetalhesDTO) ====
            CreateMap<Usuario, EventoDetalhesUsuarioDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.UserName)); 

            // ==== Naturezas ====
            CreateMap<Natureza, NaturezaResumoDTO>(); 
            CreateMap<Natureza, NaturezaDTO>();
            CreateMap<CreateNaturezaDTO, Natureza>();

            // ==== Anexos ====
            CreateMap<Models.Anexo, AnexoDTO>();

            // ==== Frames ====
            CreateMap<CreateOrEditFrameDTO, Frame>(); // Para Criar/Editar
            CreateMap<Frame, FrameDTO>(); // Para Lista Simples
            CreateMap<Frame, FrameDetalhesDTO>(); // Para Detalhes (inclui Stages)
            CreateMap<Stage, StageDTO>() // --- Mapeamentos do Stage ---
                .ForMember(dest => dest.Eventos, opt => opt.MapFrom(src => src.Eventos));

            // ==== Stages ====
            CreateMap<Stage, StageDTO>()
                .ForMember(dest => dest.Eventos, opt => opt.MapFrom(src => src.Eventos));
            CreateMap<CreateOrEditStageDTO, Stage>(); 
            CreateMap<CreateOrEditStageDTO, Stage>()
                .ForMember(dest => dest.FrameId, opt => opt.Ignore())
                .ForMember(dest => dest.Position, opt => opt.Ignore());
        }
    }
}