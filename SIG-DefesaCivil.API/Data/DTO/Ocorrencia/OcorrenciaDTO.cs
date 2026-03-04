using SIG_DefesaCivil.API.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_DefesaCivil.API.Data.DTO.Ocorrencia
{
    // --- DTO de Entrada (Create/Update) ---
    public class CreateOrEditOcorrenciaDTO : IValidatableObject
    {
        [Required(ErrorMessage = "O Tipo de Cadastro (Urgente, Basico, Completo) é obrigatório.")]
        public ETipoCadastroOcorrencia TipoCadastro { get; set; }

        [Required(ErrorMessage = "Os campos da ocorrência devem ser fornecidos.")]
        public OcorrenciaCamposDTO Campos { get; set; }

        // Relacionamentos
        public string? OcorrenciaPaiId { get; set; }
        public List<string> SubOcorrenciasId { get; set; } = new();
        public List<string> NaturezasId { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Regra Comum: Ocorrência Pai não pode ser Filha
            if (!string.IsNullOrWhiteSpace(OcorrenciaPaiId) && SubOcorrenciasId.Contains(OcorrenciaPaiId))
            {
                yield return new ValidationResult(
                    $"A Ocorrência Pai não pode ser listada como sub-ocorrência.",
                    new[] { nameof(OcorrenciaPaiId), nameof(SubOcorrenciasId) });
            }

            // Regras Específicas por Tipo de Cadastro
            switch (TipoCadastro)
            {
                case ETipoCadastroOcorrencia.Urgente:
                    if (Campos.DataEHoraDoOcorrido == null)
                        yield return new ValidationResult("Data do ocorrido é obrigatória em chamados urgentes.", new[] { "Campos.DataEHoraDoOcorrido" });

                    if (string.IsNullOrWhiteSpace(Campos.Localizacao.Rua) && string.IsNullOrWhiteSpace(Campos.Localizacao.CEP))
                        yield return new ValidationResult("É necessário fornecer Rua ou CEP para chamados urgentes.", new[] { "Campos.Endereco" });
                    break;

                case ETipoCadastroOcorrencia.Completa:
                    if (string.IsNullOrWhiteSpace(Campos.GrauDeRisco))
                        yield return new ValidationResult("O Grau de Risco é obrigatório no cadastro completo.", new[] { "Campos.GrauDeRisco" });

                    if (string.IsNullOrWhiteSpace(Campos.SolicitanteNome))
                        yield return new ValidationResult("O Nome do Solicitante é obrigatório.", new[] { "Campos.SolicitanteNome" });

                    if (!Campos.TipoDeRisco.Any())
                        yield return new ValidationResult("Pelo menos um Tipo de Risco deve ser informado.", new[] { "Campos.TipoDeRisco" });
                    break;

                case ETipoCadastroOcorrencia.Basica:
                    break;
            }
        }
    }
    // --- DTO de Saída (Detalhes Completo) ---
    public class OcorrenciaOffilineDTO
    {
        public string Id { get; set; }
        public string Numero { get; set; }
        public bool isVisible { get; set; }
        public DateTime? DataEntradaNaFaseAtual { get; set; }
        public DetalhesUsuarioDTO UsuarioCriador { get; set; }

        public OcorrenciaCamposDTO Campos { get; set; }

        public OcorrenciaPreviewDTO? OcorrenciaPai { get; set; }
        public List<OcorrenciaPreviewDTO> SubOcorrencias { get; set; } = new();
        public List<AnexoDTO> Anexos { get; set; } = new();
        public List<NaturezaResumoDTO> Naturezas { get; set; } = new();
    }

    // --- DTO de Preview (Cards do Kanban) --- //Avaliar remoção
    public class OcorrenciaPreviewDTO
    {
        public string Id { get; set; }
        public string UsuarioCriadorId { get; set; }
        public string Numero { get; set; }
        public bool isVisible { get; set; } = true;
        public string? EmailResponsavel { get; set; }

        // Dados resumidos (Flattened)
        public string? EnderecoResumido { get; set; }
        public string? SolicitanteNome { get; set; }
        public string? SolicitanteCPF { get; set; }

        public List<string> TipoDeRisco { get; set; } = new List<string>();
        public string GrauDeRisco { get; set; }
        public DateTime? DataEHoraDoOcorrido { get; set; }
    }
}