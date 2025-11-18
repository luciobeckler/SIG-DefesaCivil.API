namespace SIG_DefesaCivil.API.Models
{
    public class Form
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public bool isAtivo { get; set; }


        // Relação com fields
        public ICollection<FieldDefinition> FieldDefinition { get; set; }
    }
}
