using Microsoft.EntityFrameworkCore;

namespace SIG_DefesaCivil.API.Data.Models.Shared
{
    [Owned]
    public class Solicitante
    {
        public string? Nome { get; set; }
        public string? CPF { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
    }
}
