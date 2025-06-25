using Microsoft.AspNetCore.Identity;

namespace SIG_DefesaCivil.API.Models
{
    public class Usuario : IdentityUser
    {
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string  CPF { get; set; }
        public string Cargo { get; set; }
        public DateOnly DataAdmissao { get; set; }
        public bool isAtivo { get; set; }
        public string? Endereco { get; set; }
        public string? DataDeNascimento { get; set; }

        // Relacionamento com eventos: Um usuário pode ser responsável por vários eventos
        public ICollection<Evento>? EventosCriados { get; set; }

    }
}
