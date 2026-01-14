using SIG_DefesaCivil.API.Data.DTO;
using SIG_DefesaCivil.API.Enums;
using SIG_DefesaCivil.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SIG_DefesaCivil.API.Data.DTO.Ocorrencia
{
    // --- DTO Base (Campos Compartilhados) ---
    public abstract class OcorrenciaDadosBaseDTO
    {
        // Datas
        public DateTime? DataEHoraDoOcorrido { get; set; }
        public DateTime? DataEHoraInicioAtendimento { get; set; }
        public DateTime? DataEHoraTerminoAtendimento { get; set; }

        // --- Endereço (Achatado) ---
        public string? EnderecoRua { get; set; }
        public string? EnderecoNumero { get; set; }
        public string? EnderecoComplemento { get; set; }
        public string? EnderecoBairro { get; set; }
        public string? EnderecoCEP { get; set; }

        // --- Solicitante (Achatado) ---
        public string? SolicitanteNome { get; set; }
        public string? SolicitanteCPF { get; set; }
        public string? SolicitanteRG { get; set; } // Novo campo

        // --- Dados Quantitativos (Agora Nullable int?) ---
        public string? PossuiIPTU { get; set; }
        public int? NumeroDeMoradias { get; set; }
        public int? NumeroDeComodos { get; set; }
        public int? NumeroDePavimentos { get; set; }
        public bool? PossuiUnidadeFamiliar { get; set; }
        public int? NumeroDeDeficientes { get; set; }
        public int? NumeroDeCriancas { get; set; }
        public int? NumeroDeAdultos { get; set; }
        public int? NumeroDeIdosos { get; set; }
    }

    // --- DTO de Entrada (Create/Update) ---
    public class CreateOrEditOcorrenciaDTO : OcorrenciaDadosBaseDTO
    {

        [Required(ErrorMessage = "O Grau de Risco é obrigatório.")]
        [EnumDataType(typeof(EGrauRisco), ErrorMessage = "Valor inválido para Grau de Risco.")]
        public string GrauDeRisco { get; set; }

        [EnumDataType(typeof(ERegimeOcupacao), ErrorMessage = "Valor inválido para Regime de Ocupação.")]
        public string? RegimeDeOcupacaoDoImovel { get; set; }

        // --- Validações de Lista de Enums ---

        [EnumList(typeof(EAnalisePreliminar))]
        public List<string>? AnalisePreliminar { get; set; } = new();

        [EnumList(typeof(ECaracterizacaoLocal))]
        public List<string>? CaracterizacaoDoLocal { get; set; } = new();

        [EnumList(typeof(ETipoEdificacao))]
        public List<string>? Edificacao { get; set; } = new();

        [EnumList(typeof(ETipoEstrutura))]
        public List<string>? Estrutura { get; set; } = new();

        [EnumList(typeof(ETipoRisco))]
        public List<string>? TipoDeRisco { get; set; } = new();

        [EnumList(typeof(ETipificacaoOcorrencia))]
        public List<string>? TipificacaoDaOcorrencia { get; set; } = new();

        [EnumList(typeof(EMotivacao))]
        public List<string>? Motivacao { get; set; } = new();

        [EnumList(typeof(EAreaAfetada))]
        public List<string>? AreasAfetadas { get; set; } = new();

        // Relacionamentos
        public string? OcorrenciaPaiId { get; set; }
        public List<string>? SubOcorrenciasId { get; set; } = new List<string>();
        public List<string>? NaturezasId { get; set; } = new List<string>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(OcorrenciaPaiId) && SubOcorrenciasId != null && SubOcorrenciasId.Contains(OcorrenciaPaiId))
            {
                yield return new ValidationResult(
                    $"A Ocorrência Pai (ID: {OcorrenciaPaiId}) não pode ser listada simultaneamente como uma sub-ocorrência.",
                    new[] { nameof(OcorrenciaPaiId), nameof(SubOcorrenciasId) }
                );
            }
        }
    }

    // --- DTO de Saída (Detalhes Completo) ---
    public class OcorrenciaDetalhesDTO : OcorrenciaDadosBaseDTO
    {
        public string Id { get; set; }
        public int Numero { get; set; }
        public bool isVisible { get; set; } = true;

        public DateTime? DataEntradaNaFaseAtual { get; set; }

        // Criador
        public DetalhesUsuarioDTO UsuarioCriador { get; set; }

        // Campos Single-Select (Retorna como string)
        public string GrauDeRisco { get; set; }
        public string? RegimeDeOcupacaoDoImovel { get; set; }

        // Campos Multi-Select (Retorna como Lista de Strings para o Front ler fácil)
        public List<string> AnalisePreliminar { get; set; } = new List<string>();
        public List<string> CaracterizacaoDoLocal { get; set; } = new List<string>();
        public List<string> Edificacao { get; set; } = new List<string>();
        public List<string> Estrutura { get; set; } = new List<string>();
        public List<string> TipoDeRisco { get; set; } = new List<string>();
        public List<string> TipificacaoDaOcorrencia { get; set; } = new List<string>();
        public List<string> Motivacao { get; set; } = new List<string>();
        public List<string> AreasAfetadas { get; set; } = new List<string>();

        // Hierarquia e Anexos
        public OcorrenciaPreviewDTO? OcorrenciaPai { get; set; }
        public List<OcorrenciaPreviewDTO> SubOcorrencias { get; set; } = new List<OcorrenciaPreviewDTO>();
        public List<AnexoDTO> Anexos { get; set; } = new List<AnexoDTO>();
        public List<NaturezaResumoDTO> Naturezas { get; set; } = new List<NaturezaResumoDTO>();
    }

    // --- DTO de Preview (Cards do Kanban) ---
    public class OcorrenciaPreviewDTO
    {
        public string Id { get; set; }
        public int Numero { get; set; }
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