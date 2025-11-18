namespace SIG_DefesaCivil.API.Models
{
    public class Frame
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Stage>? Stages { get; set; }

    }
}
