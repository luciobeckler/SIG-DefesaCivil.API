using SIG_DefesaCivil.API.DTO.Eventos;
using System.ComponentModel.DataAnnotations;


namespace SIG_DefesaCivil.API.DTO.Etapas
{
    public class EtapaDTO : EtapaBaseDTO
    {
        public string Id { get; set; }

        // Inclui os ocorrencias (cartões) para renderizar o quadro
        public List<OcorrenciaPreviewDTO> Ocorrencias { get; set; } = new List<OcorrenciaPreviewDTO>();
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
        public List<string>? EtapasDestinoId { get; set; } = new List<string>();
        public List<string>? PermissoesParaTransicionarParaEstaEtapa { get; set; } = new List<string>();
    }
}