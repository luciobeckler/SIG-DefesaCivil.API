

using SIG_DefesaCivil.API.Enums;

namespace SIG_DefesaCivil.API.Models.Ocorrencia
{
    public class Ocorrencia
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Numero { get; set; }
        public bool isVisible { get; set; } = true;
        public string UsuarioCriadorId { get; set; }
        public Usuario UsuarioCriador { get; set; }
        public string EtapaId { get; set; }
        public Etapa Etapa { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DataEHoraDoOcorrido { get; set; }
        public DateTime? DataEHoraInicioAtendimento { get; set; }
        public DateTime? DataEHoraTerminoAtendimento { get; set; }
        public string? EnderecoRua { get; set; }
        public string? EnderecoNumero { get; set; }
        public string? EnderecoComplemento { get; set; }
        public string? EnderecoBairro { get; set; }
        public string? EnderecoCEP { get; set; }
        public string? SolicitanteNome { get; set; }
        public string? SolicitanteCPF { get; set; }
        public string? SolicitanteRG { get; set; }

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

        public DateTime? DataEntradaNaFaseAtual { get; set; } = DateTime.Now;
        public string? OcorrenciaPaiId { get; set; }
        public Ocorrencia? OcorrenciaPai { get; set; }
        public List<Ocorrencia> SubOcorrencias { get; set; } = new List<Ocorrencia>();
        public List<Natureza> Naturezas { get; set; } = new List<Natureza>();
        public List<Anexo> Anexos { get; set; } = new List<Anexo>();
    }
}