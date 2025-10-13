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
        public string Status { get; set; }
        public string EventoPaiId { get; set; }
        public DateTime DataEHoraDoEvento { get; set; }

        public EventoDetalhesUsuarioDTO UsuarioCriador { get; set; }
        public ICollection<EventoDetalhesSubEventoDTO> SubEventos { get; set; }
    }
}
