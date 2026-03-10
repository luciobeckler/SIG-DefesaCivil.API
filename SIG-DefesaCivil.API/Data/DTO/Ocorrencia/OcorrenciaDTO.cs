using SIG_DefesaCivil.API.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_DefesaCivil.API.Data.DTO.Ocorrencia
{
    // --- DTO de Saída (Detalhes Completo) ---
    public class OcorrenciaDTO
    {
        public string Id { get; set; }
        public string Protocolo { get; set; }
        public bool isVisible { get; set; }
        public DateTime? DataEntradaNaFaseAtual { get; set; }
        public ResponsavelDTO? Responsavel { get; set; }

        public OcorrenciaCamposDTO Campos { get; set; }

        public List<AnexoDTO> Anexos { get; set; } = new();
    }

    // --- DTO de Entrada (Create/Update) ---
    public class CreateOrEditOcorrenciaDTO : IValidatableObject
    {
        public ETipoCadastroOcorrencia TipoCadastro { get; set; }

        public OcorrenciaCamposDTO Campos { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IEnumerable<ValidationResult> errors = TipoCadastro switch
            {
                ETipoCadastroOcorrencia.Urgente => ValidaCasoUrgente(Campos),
                ETipoCadastroOcorrencia.Completa => ValidaCasoCompleta(Campos),
                ETipoCadastroOcorrencia.Basica => ValidaCasoBasica(Campos),
                ETipoCadastroOcorrencia.SolicitacaoVistoria => ValidaCasoSolicitacaoVistoria(Campos),
                _ => Enumerable.Empty<ValidationResult>()
            };

            foreach (var error in errors) yield return error;
        }

        private IEnumerable<ValidationResult> ValidaCasoSolicitacaoVistoria(OcorrenciaCamposDTO campos)
        {
            if (string.IsNullOrEmpty(Campos.Solicitante.Nome))
                yield return new ValidationResult("Nome faltando.");
            if (string.IsNullOrEmpty(Campos.Solicitante.CPF))
                yield return new ValidationResult("CPF faltando.");
            if (string.IsNullOrEmpty(Campos.Solicitante.Email))
                yield return new ValidationResult("Email faltando.");
            if (string.IsNullOrEmpty(Campos.Solicitante.Telefone))
                yield return new ValidationResult("Telefone faltando.");

            if (string.IsNullOrEmpty(Campos.Localizacao.Rua) && string.IsNullOrEmpty(Campos.Localizacao.CEP))
                yield return new ValidationResult("É necessário fornecer Rua ou CEP para solicitações de vistoria.");
        }

        private IEnumerable<ValidationResult> ValidaCasoBasica(OcorrenciaCamposDTO campos)
        {
            if (string.IsNullOrWhiteSpace(Campos.GrauDeRisco))
                yield return new ValidationResult("O Grau de Risco é obrigatório no cadastro completo.", new[] { "Campos.GrauDeRisco" });

            if (string.IsNullOrWhiteSpace(Campos.DataEHoraDoOcorrido.ToString()))
                yield return new ValidationResult("A data e hora do ocorrido é obrigatório no cadastro básico.", new[] { "Campos.DataEHoraDoOcorrido" });
        }

        private IEnumerable<ValidationResult> ValidaCasoCompleta(OcorrenciaCamposDTO campos)
        {
            if (string.IsNullOrWhiteSpace(Campos.GrauDeRisco))
                yield return new ValidationResult("O Grau de Risco é obrigatório no cadastro completo.", new[] { "Campos.GrauDeRisco" });

            if (string.IsNullOrWhiteSpace(Campos.Solicitante.Nome))
                yield return new ValidationResult("O Nome do Solicitante é obrigatório.", new[] { "Campos.SolicitanteNome" });

            if (!Campos.TipoDeRisco.Any())
                yield return new ValidationResult("Pelo menos um Tipo de Risco deve ser informado.", new[] { "Campos.TipoDeRisco" });
        }

        private IEnumerable<ValidationResult> ValidaCasoUrgente(OcorrenciaCamposDTO campos)
        {
            if (Campos.DataEHoraDoOcorrido == null)
                yield return new ValidationResult("Data do ocorrido é obrigatória em chamados urgentes.", new[] { "Campos.DataEHoraDoOcorrido" });

            if (string.IsNullOrWhiteSpace(Campos.Localizacao.Rua) && string.IsNullOrWhiteSpace(Campos.Localizacao.CEP))
                yield return new ValidationResult("É necessário fornecer Rua ou CEP para chamados urgentes.", new[] { "Campos.Endereco" });
        }
    }
}