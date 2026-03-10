using System.ComponentModel.DataAnnotations;

namespace SIG_DefesaCivil.API.Data.DTO
{
    public class QuadroDTO
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public List<EtapaDTO> Etapas { get; set; } = new List<EtapaDTO>();
    }

    public class CriarOuEditarQuadroDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
}