using SIG_DefesaCivil.API.Data.Models.Shared;

namespace SIG_DefesaCivil.API.Data.DTO
{
    public class SolicitacaoVistoriaDTO
    {
        public Solicitante Solicitante { get; set; } = new Solicitante();
        public Endereco Localizacao { get; set; } = new Endereco();
    }
}