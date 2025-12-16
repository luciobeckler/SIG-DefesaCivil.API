// SIG_DefesaCivil.API/Models/Natureza.cs

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
        public List<Natureza>? SubNaturezas { get; set; } = new List<Natureza>();

        // N-N com ocorrencias
        public List<Ocorrencia.Ocorrencia> Ocorrencias { get; set; } = new List<Ocorrencia.Ocorrencia>();
    }
}