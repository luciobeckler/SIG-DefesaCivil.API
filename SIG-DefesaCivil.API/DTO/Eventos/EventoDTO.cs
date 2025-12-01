using SIG_DefesaCivil.API.DTOs;
using SIG_DefesaCivil.API.Enums;

namespace SIG_DefesaCivil.API.DTO.Eventos
{
    public abstract class EventoDadosBaseDTO
    {
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Endereco { get; set; }
        public DateTime DataEHoraDoEvento { get; set; }
        public DateTime DataEntradaNaFaseAtual { get; set; }
    }

    public class CreateOrEditEventoDTO : EventoDadosBaseDTO
    {

        public string Status { get; set; }

        public string? EventoPaiId { get; set; }
        public ICollection<string>? SubEventosId { get; set; } = new List<string>();
        public ICollection<string>? NaturezasId { get; set; } = new List<string>();
    }
    public class EventoDetalhesDTO : EventoDadosBaseDTO
    {
        public string Id { get; set; }
        public EStatusEvento Status { get; set; }

        public EventoPreviewDTO EventoPai { get; set; }
        public DetalhesUsuarioDTO UsuarioCriador { get; set; }
        public ICollection<EventoPreviewDTO> SubEventos { get; set; } = new List<EventoPreviewDTO>();
        public ICollection<NaturezaResumoDTO> Naturezas { get; set; } = new List<NaturezaResumoDTO>();
        public ICollection<AnexoDTO> Anexos { get; set; } = new List<AnexoDTO>();
        public bool isVisible { get; set; }
    }
    public class EventoPreviewDTO
    {
        public string Id { get; set; }
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public EStatusEvento Status { get; set; }
        public string EmailResponsavel { get; set; }
        public ICollection<NaturezaResumoDTO> Naturezas { get; set; } = new List<NaturezaResumoDTO>();
        public bool isVisible { get; set; }
    }

    public class DetalhesUsuarioDTO
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}
