using SIG_DefesaCivil.API.DTO;
using System.ComponentModel.DataAnnotations;

namespace SIG_DefesaCivil.API.DTOs.Frames
{
    public class CreateOrEditFrameDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class FrameDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class FrameDetalhesDTO : FrameDTO
    {
        public ICollection<StageDTO> Stages { get; set; }
    }
}