namespace SIG_DefesaCivil.API.Models
{
    public class FormCompleted
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string FormId { get; set; } 
        public Form Formulario { get; set; }

        public string UsuarioId { get; set; } 
        public Usuario Usuario { get; set; }

        public DateTime DataCompleted { get; set; } = DateTime.UtcNow;


        public ICollection<FieldResponse> Responses { get; set; }
    }
}
