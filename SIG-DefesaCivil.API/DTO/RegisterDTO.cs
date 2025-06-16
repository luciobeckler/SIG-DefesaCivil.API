namespace SIG_DefesaCivil.API.DTO
{
    public class RegisterDTO : LoginDTO
    {
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string CPF { get; set; }
        public DateOnly DataAdmissao { get; set; }
        public string Cargo { get; set; }
        public bool IsAtivo { get; set; } = true; // Por padrão, o usuário está ativo
    }
}
