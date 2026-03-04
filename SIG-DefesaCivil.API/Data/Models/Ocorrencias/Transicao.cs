using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Ocorrencia;

namespace SIG_DefesaCivil.API.Data.Models.Ocorrencias
{
    public class Transicao
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime DataEHorario { get; set; }

        public string OcorrenciaId { get; set; }
        public Ocorrencia Ocorrencia { get; set; }
        public string ResponsavelId { get; set; }
        public Usuario Responsavel { get; set; }
        public string EtapaAtualId { get; set; }
        public Etapa EtapaAtual { get; set; }
        public string EtapaAnteriorId { get; set; }
        public Etapa EtapaAnterior { get; set; }
    }
}
