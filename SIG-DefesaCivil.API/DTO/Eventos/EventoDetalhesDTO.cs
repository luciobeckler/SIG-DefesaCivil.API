using SIG_DefesaCivil.API.Enums;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.DTO.Eventos
{
    public class EventoDetalhesDTO
    {
        public string Id { get; set; }
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Endereco { get; set; }
        public EStatusEvento Status { get; set; }
        public EventoPreviewDTO EventoPai { get; set; }
        public DateTime DataEHoraDoEvento { get; set; }

        public EventoDetalhesUsuarioDTO UsuarioCriador { get; set; }
        public ICollection<EventoPreviewDTO> SubEventos { get; set; }
    }
}
