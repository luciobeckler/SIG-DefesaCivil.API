using SIG_DefesaCivil.API.DTOs;
using SIG_DefesaCivil.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_DefesaCivil.API.DTO.Eventos
{
    public abstract class OcorrenciaDadosBaseDTO
    {
        public string Numero { get; set; } // Protocolo

        public DateTime DataEHoraDaSolicitacao { get; set; }
        public DateTime? DataEHoraInicioAtendimento { get; set; }
        public DateTime? DataEHoraTerminoAtendimento { get; set; }

        // Campos Simples
        public string PossuiIPTU { get; set; }
        public int NumeroDeMoradias { get; set; }
        public int NumeroDeComodos { get; set; }
        public int NumeroDePavimentos { get; set; }
        public bool PossuiUnidadeFamiliar { get; set; }
        public int NumeroDeDeficientes { get; set; }
        public int NumeroDeCriancas { get; set; }
        public int NumeroDeAdultos { get; set; }
        public int NumeroDeIdosos { get; set; }
    }

    public class CreateOrEditOcorrenciaDTO : OcorrenciaDadosBaseDTO
    {
        [Required]
        public string StageId { get; set; }
        public CivilDTO Solicitante { get; set; }
        public EnderecoDTO Endereco { get; set; }

        // Campos Single-Select (Recebe como string do Front)
        public string GrauDeRisco { get; set; }
        public string RegimeDeOcupacaoDoImovel { get; set; }

        // Campos Multi-Select (Recebe como Lista de Strings)
        public List<string> AnalisePreliminar { get; set; } = new();
        public List<string> CaracterizacaoDoLocal { get; set; } = new();
        public List<string> Edificacao { get; set; } = new();
        public List<string> Estrutura { get; set; } = new();
        public List<string> TipoDeRisco { get; set; } = new();
        public List<string> TipificacaoDaOcorrencia { get; set; } = new();
        public List<string> Motivacao { get; set; } = new();
        public List<string> AreasAfetadas { get; set; } = new();

        // Relacionamentos de Hierarquia
        public string? EventoPaiId { get; set; }
        public ICollection<string>? SubEventosId { get; set; } = new List<string>();

        // Se ainda usar Naturezas legado
        public ICollection<string>? NaturezasId { get; set; } = new List<string>();
    }
    public class OcorrenciaDetalhesDTO : OcorrenciaDadosBaseDTO
    {
        public string Id { get; set; }
        public bool isVisible { get; set; }
        public DateTime DataEntradaNaFaseAtual { get; set; }

        // Kanban
        public string StageId { get; set; }
        public string StageName { get; set; }

        // Entidades Relacionadas
        public CivilDTO Solicitante { get; set; }
        public EnderecoDTO Endereco { get; set; }
        public DetalhesUsuarioDTO UsuarioCriador { get; set; }

        // Campos Single-Select (Retorna como string)
        public string GrauDeRisco { get; set; }
        public string RegimeDeOcupacaoDoImovel { get; set; }

        // Campos Multi-Select (Retorna como Lista de Strings)
        public List<string> AnalisePreliminar { get; set; } = new();
        public List<string> CaracterizacaoDoLocal { get; set; } = new();
        public List<string> Edificacao { get; set; } = new();
        public List<string> Estrutura { get; set; } = new();
        public List<string> TipoDeRisco { get; set; } = new();
        public List<string> TipificacaoDaOcorrencia { get; set; } = new();
        public List<string> Motivacao { get; set; } = new();
        public List<string> AreasAfetadas { get; set; } = new();

        // Hierarquia e Anexos
        public OcorrenciaPreviewDTO? EventoPai { get; set; }
        public ICollection<OcorrenciaPreviewDTO> SubEventos { get; set; } = new List<OcorrenciaPreviewDTO>();
        public ICollection<AnexoDTO> Anexos { get; set; } = new List<AnexoDTO>();

        // Legado se manter
        public ICollection<NaturezaResumoDTO> Naturezas { get; set; } = new List<NaturezaResumoDTO>();
    }
    public class OcorrenciaPreviewDTO
    {
        public string Id { get; set; }
        public string Numero { get; set; }
        public string StageId { get; set; }
        public string StageName { get; set; }
        public bool isVisible { get; set; }
        public string EmailResponsavel { get; set; }

        // Dados resumidos para o card
        public string EnderecoResumido { get; set; } // Ex: "Rua X, Bairro Y"
        public string SolicitanteNome { get; set; }
        public string CPF { get; set; }
        public List<string> TipoDeRisco { get; set; }
    }

    public class DetalhesUsuarioDTO
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}
