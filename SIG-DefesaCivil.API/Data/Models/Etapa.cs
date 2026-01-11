using SIG_DefesaCivil.API.Enums;

namespace SIG_DefesaCivil.API.Models
{
    public class Etapa
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public int Posicao { get; set; } // Para ordenar as colunas (1, 2, 3...)

        // Regras de etapa
        public TimeSpan? MinTempoNaEtapa { get; set; } = TimeSpan.MinValue;
        public TimeSpan? MaxTempoNaEtapa { get; set; } = TimeSpan.MaxValue;
        public List<string>? EtapasDestinoId { get; set; } = new List<string>();
        public List<ECargos> PermissoesParaTransicionarParaEstaEtapa { get; set; } = new List<ECargos>();

        // FK para o Quadro
        public string QuadroId { get; set; }
        public Quadro Quadro { get; set; }

        // Uma Etapa tem vários Eventos (Cartões)
        public List<Ocorrencia.Ocorrencia> Ocorrencias { get; set; } = new List<Ocorrencia.Ocorrencia>();
    }
}
