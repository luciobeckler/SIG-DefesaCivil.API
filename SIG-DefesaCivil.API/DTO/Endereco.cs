namespace SIG_DefesaCivil.API.DTO
{
    public class EnderecoDTO
    {
        public string? Id { get; set; } // Opcional na criação
        public string Rua { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string CEP { get; set; }
    }
}
