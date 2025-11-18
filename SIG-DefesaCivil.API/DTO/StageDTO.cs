using SIG_DefesaCivil.API.DTO.Eventos.SIG_DefesaCivil.API.DTO.EventoDTO;
using System.ComponentModel.DataAnnotations;

namespace SIG_DefesaCivil.API.DTO
{
    public class StageDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Position { get; set; }
        public string? FormularioId { get; set; } 

        public ICollection<EventoPreviewDTO> Eventos { get; set; }
    }

    public class CreateOrEditStageDTO
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        public string? FormularioId { get; set; }

        [Required]
        public string FrameId { get; set; }
    }

    public class ReorderStagesDTO
    {
        public List<string> StageIdsInOrder { get; set; }
    }
}
