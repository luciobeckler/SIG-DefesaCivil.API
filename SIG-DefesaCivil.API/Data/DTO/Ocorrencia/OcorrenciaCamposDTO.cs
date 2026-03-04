using SIG_DefesaCivil.API.Data.Enums;
using SIG_DefesaCivil.API.Data.Models.Shared;
using SIG_DefesaCivil.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SIG_DefesaCivil.API.Data.DTO.Ocorrencia
{
    public class OcorrenciaCamposDTO
    {
        public DateTime? DataEHoraDoOcorrido { get; set; }
        public DateTime? DataEHoraInicioAtendimento { get; set; }
        public DateTime? DataEHoraTerminoAtendimento { get; set; }

        public Endereco? Localizacao { get; set; }

        public string? SolicitanteNome { get; set; }
        public string? SolicitanteCPF { get; set; }
        public string? SolicitanteRG { get; set; }

        // Classificações
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

        // Quantitativos
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
}
