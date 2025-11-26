using SIG_DefesaCivil.API.DTO.Eventos;
using System.ComponentModel.DataAnnotations;


namespace SIG_DefesaCivil.API.DTO.Etapas
{
    public class EtapaDTO : EtapaBaseDTO
    {
        public string Id { get; set; }

        // Inclui os eventos (cartões) para renderizar o quadro
        public ICollection<EventoPreviewDTO> Eventos { get; set; }
    }

    public class CriaOuAtualizaEtapaDTO : EtapaBaseDTO
    {
        
    }

    public class ReordenarEtapaDTO
    {
        // Lista de IDs na nova ordem desejada
        public List<string> IdsDasEtapasNaOrdem { get; set; }
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
        public int? MinSegundosNaEtapa { get; set; } = 0;
        public int? MaxSegundosNaEtapa { get; set; }
        public ICollection<string>? EtapasDestinoId { get; set; }
        public ICollection<string>? PermissoesParaTransicionarParaEstaEtapa { get; set; }
    }
}