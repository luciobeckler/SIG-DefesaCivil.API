namespace SIG_DefesaCivil.API.DTO
{
    public class CivilDTO
    {
        public string? Id { get; set; } // Opcional na criação se for novo
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string CI { get; set; }
        public string Telefone { get; set; }
    }
}
