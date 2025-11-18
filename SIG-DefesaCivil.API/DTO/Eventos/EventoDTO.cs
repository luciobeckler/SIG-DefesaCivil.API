using SIG_DefesaCivil.API.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SIG_DefesaCivil.API.DTO.Eventos
{

    namespace SIG_DefesaCivil.API.DTO.EventoDTO
    {
        /// <summary>
        /// Classe base com os dados essenciais de um evento.
        /// </summary>
        public abstract class EventoDadosBaseDTO
        {
            public string Codigo { get; set; }
            public string Titulo { get; set; }
            public string Descricao { get; set; }
            public string Endereco { get; set; }
            public DateTime DataEHoraDoEvento { get; set; }
        }

        public class CreateEventoDTO : EventoDadosBaseDTO
        {
            [Required]
            public string StageId { get; set; } 

            public string? EventoPaiId { get; set; }
            public ICollection<string>? SubEventosId { get; set; }
            public ICollection<string>? NaturezasId { get; set; }
        }

        public class UpdateEventoDTO : EventoDadosBaseDTO
        {
            public string? EventoPaiId { get; set; }
            public ICollection<string>? SubEventosId { get; set; }
            public ICollection<string>? NaturezasId { get; set; }

            public bool isVisible { get; set; }
        }

        public class EventoPreviewDTO
        {
            public string Id { get; set; }
            public string Codigo { get; set; }
            public string Titulo { get; set; }
            public bool isVisible { get; set; }
            public string EmailResponsavel { get; set; }

            public string StageId { get; set; }
            public ICollection<NaturezaResumoDTO> Naturezas { get; set; }
        }

        public class EventoDetalhesDTO : EventoDadosBaseDTO
        {
            public string Id { get; set; }
            public bool isVisible { get; set; }
            public string StageId { get; set; }

            // --- RELACIONAMENTOS ---
            public EventoPreviewDTO? EventoPai { get; set; }
            public EventoDetalhesUsuarioDTO UsuarioCriador { get; set; }
            public ICollection<EventoPreviewDTO> SubEventos { get; set; }
            public ICollection<NaturezaResumoDTO> Naturezas { get; set; }
            public ICollection<AnexoDTO> Anexos { get; set; }
        }

        public class EventoDetalhesUsuarioDTO
        {
            public string Id { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }
        }
    }
}
