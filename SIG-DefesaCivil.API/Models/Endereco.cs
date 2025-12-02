namespace SIG_DefesaCivil.API.Models
{
    public class Endereco
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Rua { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string CEP { get; set; }
    }
}
