using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.Models
{
    public class Stage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public float Position { get; set; }

        // Relacionamento com Quadro
        public string FrameId { get; set; }
        public Frame Frame { get; set; }

        // Relacionamento com forms
        public string? FormId { get; set; }
        public Form? Form { get; set; }

        public ICollection<Evento>? Eventos { get; set; }
    }
}
