using Microsoft.EntityFrameworkCore;

namespace SIG_DefesaCivil.API.Data.Models.Shared
{
    [Owned]
    public class Endereco
    {
        public string? Rua { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? CEP { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}
