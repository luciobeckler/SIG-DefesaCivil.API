using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Data.Enums;

namespace SIG_DefesaCivil.API.Data.Models.Shared
{
    [Owned]
    public class OcorrenciaCampos
    {
        public DateTime? DataEHoraDoOcorrido { get; set; }
        public DateTime? DataEHoraInicioAtendimento { get; set; }
        public DateTime? DataEHoraTerminoAtendimento { get; set; }
        public Endereco? Localizacao { get; set; }
        public Solicitante? Solicitante { get; set; }
        public List<EAnalisePreliminar> AnalisePreliminar { get; set; } = new List<EAnalisePreliminar>();
        public List<ECaracterizacaoLocal> CaracterizacaoDoLocal { get; set; } = new List<ECaracterizacaoLocal>();
        public List<ETipoEdificacao> Edificacao { get; set; } = new List<ETipoEdificacao>();
        public List<ETipoEstrutura> Estrutura { get; set; } = new List<ETipoEstrutura>();
        public List<ETipoRisco> TipoDeRisco { get; set; } = new List<ETipoRisco>();
        public List<ETipificacaoOcorrencia> TipificacaoDaOcorrencia { get; set; } = new List<ETipificacaoOcorrencia>();
        public List<EMotivacao> Motivacao { get; set; } = new List<EMotivacao>();
        public List<EAreaAfetada> AreasAfetadas { get; set; } = new List<EAreaAfetada>();

        public EGrauRisco? GrauDeRisco { get; set; }
        public ERegimeOcupacao? RegimeDeOcupacaoDoImovel { get; set; }

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
