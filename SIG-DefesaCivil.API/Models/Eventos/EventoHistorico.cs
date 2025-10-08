using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.Models
{
    public class EventoHistorico
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EventoId { get; set; }
        public Evento Evento { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public string Acao { get; set; } // "Visualizou detalhes" ou "Editou de evento"
        public DateTime UltimaAlteracao { get; set; } = DateTime.UtcNow;
    }
}
