using SIG_DefesaCivil.API.Data.DTO.Ocorrencia;
using System.ComponentModel.DataAnnotations;


namespace SIG_DefesaCivil.API.Data.DTO
{
    public class EtapaDTO : EtapaBaseDTO
    {
        public string Id { get; set; }

        // Inclui os ocorrencias (cartões) para renderizar o quadro
        public List<OcorrenciaOffilineDTO> Ocorrencias { get; set; } = new List<OcorrenciaOffilineDTO>();
    }

    public class CriaOuAtualizaEtapaDTO : EtapaBaseDTO
    {

    }

    public class ReordenarEtapaDTO
    {
        // Lista de IDs na nova ordem desejada
        public List<string> IdsDasEtapasNaOrdem { get; set; } = new List<string>();
    }

    public class EtapaBaseDTO : RegrasDeTransicaoEtapaDTO
    {
        [Required]
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Posicao { get; set; }
        [Required]
        public string QuadroId { get; set; }
    }

    public class RegrasDeTransicaoEtapaDTO
    {
        public TimeSpan? MinTempoNaEtapa { get; set; } = TimeSpan.MinValue;
        public TimeSpan? MaxTempoNaEtapa { get; set; } = TimeSpan.MaxValue;
        public List<string>? EtapasDestinoId { get; set; } = new List<string>();
        public List<string>? PermissoesParaTransicionarParaEstaEtapa { get; set; } = new List<string>();
    }
}