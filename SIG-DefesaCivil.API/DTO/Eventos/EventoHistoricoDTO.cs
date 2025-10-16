using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.DTO.Eventos
{
    public class EventoHistoricoDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EventoId { get; set; }
        public string UsuarioId { get; set; }
        public string Acao { get; set; } // "Visualizou detalhes" ou "Editou de evento"
        public DateTime UltimaAlteracao { get; set; } = DateTime.UtcNow;
    }
}
