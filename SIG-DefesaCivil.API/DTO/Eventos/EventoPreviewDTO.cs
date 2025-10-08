using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.DTO.Eventos
{
    public class EventoPreviewDTO
    {
        public string id { get; set; }
        public string codigo { get; set; }
        public string titulo { get; set; }
        public string status { get; set; }
        public string emailResponsavel { get; set; }

    }
}
