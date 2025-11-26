using System.ComponentModel.DataAnnotations;

namespace SIG_DefesaCivil.API.DTO.Quadros
{
    public class QuadroDTO
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }

    public class CriarOuEditarQuadroDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }

    public class QuadroDetalhesDTO : QuadroDTO
    {
        public ICollection<Etapas.EtapaDTO> Etapas { get; set; }
    }
}