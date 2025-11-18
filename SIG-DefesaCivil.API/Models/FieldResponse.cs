namespace SIG_DefesaCivil.API.Models
{
    public class FieldResponse
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string FormCompletedId { get; set; } // FK para o "preenchimento"
        public FormCompleted FormCompleted { get; set; }

        public string FieldDefinitionId { get; set; } // FK para a "pergunta"
        public FieldDefinition FieldDefinition { get; set; }

        /// <summary>
        /// O valor da resposta, sempre armazenado como texto.
        /// - Text, Number, Date, Boolean, Select: "O valor em si"
        /// - MultiSelect: "[\"Valor1\", \"Valor2\"]" (JSON serializado)
        /// - File: null (o arquivo é gerenciado pelo Anexo.cs)
        /// </summary>
        public string? Value { get; set; }
    }
}
