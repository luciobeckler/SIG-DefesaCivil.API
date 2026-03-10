using SIG_DefesaCivil.API.Data.DTO.Ocorrencia;
using System.ComponentModel.DataAnnotations;


namespace SIG_DefesaCivil.API.Data.DTO
{
    public class EtapaDTO : CriaOuAtualizaEtapaDTO
    {
        public string Id { get; set; }

        public List<OcorrenciaDTO> Ocorrencias { get; set; } = new List<OcorrenciaDTO>();
    }

    public class CriaOuAtualizaEtapaDTO : RegrasDeTransicaoEtapaDTO
    {
        [Required]
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Posicao { get; set; }
        [Required]
        public string QuadroId { get; set; }
    }

    public class ReordenarEtapaDTO
    {
        // Lista de IDs na nova ordem desejada
        public List<string> IdsDasEtapasNaOrdem { get; set; } = new List<string>();
    }


    public class RegrasDeTransicaoEtapaDTO
    {
        public TimeSpan? MinTempoNaEtapa { get; set; } = TimeSpan.MinValue;
        public TimeSpan? MaxTempoNaEtapa { get; set; } = TimeSpan.MaxValue;
        public List<string>? EtapasDestinoId { get; set; } = new List<string>();
        public List<string>? PermissoesParaTransicionarParaEstaEtapa { get; set; } = new List<string>();
    }
}