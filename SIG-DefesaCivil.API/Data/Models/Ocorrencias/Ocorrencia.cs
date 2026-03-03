

using SIG_DefesaCivil.API.Data.Models.Ocorrencias;

namespace SIG_DefesaCivil.API.Models.Ocorrencia
{
    public class Ocorrencia
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Numero { get; set; }
        public bool isVisible { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OcorrenciaCampos Campos { get; set; }

        //Relacionamentos
        public DateTime? DataEntradaNaFaseAtual { get; set; } = DateTime.Now;
        public string? OcorrenciaPaiId { get; set; }
        public Ocorrencia? OcorrenciaPai { get; set; }
        public List<Ocorrencia> SubOcorrencias { get; set; } = new List<Ocorrencia>();
        public List<Natureza> Naturezas { get; set; } = new List<Natureza>();
        public List<Anexo> Anexos { get; set; } = new List<Anexo>();
        public string UsuarioCriadorId { get; set; }
        public Usuario UsuarioCriador { get; set; }
        public string EtapaId { get; set; }
        public Etapa Etapa { get; set; }
    }
}