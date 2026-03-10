using SIG_DefesaCivil.API.Data.Models.Ocorrencias;
using SIG_DefesaCivil.API.Data.Models.Shared;

namespace SIG_DefesaCivil.API.Models.Ocorrencia
{
    public class Ocorrencia
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Protocolo { get; set; }
        public bool isVisivel { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OcorrenciaCampos Campos { get; set; } = new OcorrenciaCampos();
        public DateTime? DataEntradaNaFaseAtual { get; set; } = DateTime.Now;


        //Relacionamentos
        public List<Anexo> Anexos { get; set; } = new List<Anexo>();
        public string? ResponsavelId { get; set; }
        public Usuario? Responsavel { get; set; }
        public string EtapaId { get; set; }
        public Etapa Etapa { get; set; }
        public List<Transicao> Transicoes { get; set; } = new List<Transicao>();
    }
}