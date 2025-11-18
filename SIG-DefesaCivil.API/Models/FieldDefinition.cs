using SIG_DefesaCivil.API.Enums;

namespace SIG_DefesaCivil.API.Models
{
    public class FieldDefinition
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FormId { get; set; } 
        public Form Form { get; set; }

        public string Label { get; set; } 
        public ETipoCampo Type { get; set; } 
        public float Position { get; set; } 
        public bool IsRequired { get; set; }

        /// <summary>
        /// Para 'Select' e 'MultiSelect', armazena as opções.
        /// (Recomendado: JSON array, ex: "[\"Alto\", \"Médio\", \"Baixo\"]")
        /// </summary>
        public string? Opcoes { get; set; }
    }
}
