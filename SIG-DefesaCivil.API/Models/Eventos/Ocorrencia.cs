// SIG_DefesaCivil.API/Models/Eventos/Evento.cs

using SIG_DefesaCivil.API.Enums;

namespace SIG_DefesaCivil.API.Models.Ocorrencia
{
    public class Ocorrencia
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Dados de Controle
        public string Numero { get; set; } // Protocolo gerado
        public DateTime DataEHoraDaSolicitacao { get; set; }
        public DateTime? DataEHoraInicioAtendimento { get; set; }
        public DateTime? DataEHoraTerminoAtendimento { get; set; }

        // Relacionamentos 1-1 (Entidades Auxiliares)
        public string SolicitanteId { get; set; }
        public Civil Solicitante { get; set; }

        public string EnderecoId { get; set; }
        public Endereco Endereco { get; set; }

        // Campos Multi-Select (Listas de Enums)
        public List<EAnalisePreliminar> AnalisePreliminar { get; set; } = new();
        public List<ECaracterizacaoLocal> CaracterizacaoDoLocal { get; set; } = new();
        public List<ETipoEdificacao> Edificacao { get; set; } = new();
        public List<ETipoEstrutura> Estrutura { get; set; } = new();
        public List<ETipoRisco> TipoDeRisco { get; set; } = new();
        public List<ETipificacaoOcorrencia> TipificacaoDaOcorrencia { get; set; } = new();
        public List<EMotivacao> Motivacao { get; set; } = new();
        public List<EAreaAfetada> AreasAfetadas { get; set; } = new();

        // Campos Single-Select e Simples
        public string PossuiIPTU { get; set; } // Mantido como string conforme solicitado
        public int NumeroDeMoradias { get; set; }
        public int NumeroDeComodos { get; set; }
        public int NumeroDePavimentos { get; set; }
        public bool PossuiUnidadeFamiliar { get; set; }
        public int NumeroDeDeficientes { get; set; }
        public int NumeroDeCriancas { get; set; }
        public int NumeroDeAdultos { get; set; }
        public int NumeroDeIdosos { get; set; }

        public EGrauRisco GrauDeRisco { get; set; } // Single Select
        public ERegimeOcupacao RegimeDeOcupacaoDoImovel { get; set; } // Single Select

        // Campos de Sistema (Kanban, Usuário, etc)
        public DateTime DataEntradaNaFaseAtual { get; set; } = DateTime.Now;
        public bool isVisible { get; set; } = true;
        public string UsuarioCriadorId { get; set; }
        public Usuario UsuarioCriador { get; set; }

        public string? OcorrenciaPaiId { get; set; }
        public Ocorrencia? OcorrenciaPai { get; set; }
        public ICollection<Ocorrencia>? SubOcorrencias { get; set; } = new List<Ocorrencia>();
        public ICollection<Natureza> Naturezas { get; set; } = new List<Natureza>(); // N-N Legado se ainda usar
        public ICollection<Anexo> Anexos { get; set; } = new List<Anexo>();

        public string EtapaId { get; set; }
        public Etapa Etapa { get; set; }

    }
}