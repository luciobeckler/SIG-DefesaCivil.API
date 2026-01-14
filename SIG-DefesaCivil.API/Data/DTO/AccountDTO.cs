namespace SIG_DefesaCivil.API.Data.DTO
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    public class RegisterDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string CPF { get; set; }
        public string Cargo { get; set; }
        public string Permissao { get; set; }
        public bool IsAtivo { get; set; } = true; // Por padrão, o usuário está ativo

        public DateOnly? DataAdmissao { get; set; }
        public string? Endereco { get; set; }
        public string? DataDeNascimento { get; set; }
    }

    public class LogoutDTO
    {
        public string RefreshToken { get; set; }
    }

    public class AlterarSenhaDTO
    {
        public string NovaSenha { get; set; }
    }
}
