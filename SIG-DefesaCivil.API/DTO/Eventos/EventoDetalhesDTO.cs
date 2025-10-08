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
        public DateTime DataHora { get; set; }

        public string UsuarioCriadorId { get; set; }
        public string? EventoPaiId { get; set; }
        public ICollection<Evento>? SubEventos { get; set; }
    }
}
