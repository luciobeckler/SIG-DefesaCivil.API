namespace SIG_DefesaCivil.API.Models
{
    public class Natureza
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string CodigoNatureza { get; set; }

        // Auto-relação: Uma natureza pode ter várias subnaturezas
        public string? NaturezaPaiId { get; set; }
        public Natureza? NaturezaPai { get; set; }
        public ICollection<Natureza>? SubNaturezas { get; set; }
    }
}
