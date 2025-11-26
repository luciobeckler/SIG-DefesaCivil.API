namespace SIG_DefesaCivil.API.Models
{
    public class Etapa
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public int Posicao { get; set; } // Para ordenar as colunas (1, 2, 3...)
        
        // Regras de etapa
        public int? MinSegundosNaEtapa { get; set; } = 0;
        public int? MaxSegundosNaEtapa { get; set; }
        public ICollection<string>? EtapasDestinoId { get; set; }
        public ICollection<string> PermissoesParaTransicionarParaEstaEtapa { get; set; } //V

        // FK para o Quadro
        public string QuadroId{ get; set; }
        public Quadro Quadro{ get; set; }

        // Uma Etapa tem vários Eventos (Cartões)
        public ICollection<Eventos.Evento> Eventos { get; set; }
    }
}
