using Microsoft.AspNetCore.Identity;
using SIG_DefesaCivil.API.Models.Ocorrencia;

namespace SIG_DefesaCivil.API.Models
{
    public class Usuario : IdentityUser
    {
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string  CPF { get; set; }
        public string Cargo { get; set; }
        public bool isAtivo { get; set; } = true;
        public bool isPrimeiroAcesso { get; set; } = true;
        
        public DateOnly? DataAdmissao { get; set; }
        public string? Endereco { get; set; }
        public string? DataDeNascimento { get; set; }

        // Relacionamento com ocorrencias: Um usuário pode ser responsável por vários ocorrencias
        public ICollection<Ocorrencia.Ocorrencia>? OcorrenciasCriados { get; set; } = new List<Ocorrencia.Ocorrencia>();

    }
}
