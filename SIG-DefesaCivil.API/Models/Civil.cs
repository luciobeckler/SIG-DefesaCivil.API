namespace SIG_DefesaCivil.API.Models
{
    public class Civil
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; } // Carteira de Identidade (RG)
        public string Telefone { get; set; }
    }
}
