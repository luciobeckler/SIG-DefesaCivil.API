// SIG_DefesaCivil.API/Models/Natureza.cs

using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.Models
{
    public class Natureza
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string CodigoNatureza { get; set; }

        // Auto-relação de hierarquia
        public string? NaturezaPaiId { get; set; }
        public Natureza? NaturezaPai { get; set; }
        public ICollection<Natureza>? SubNaturezas { get; set; } = new List<Natureza>();

        // N-N com eventos
        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    }
}