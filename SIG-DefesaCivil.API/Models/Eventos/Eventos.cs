// SIG_DefesaCivil.API/Models/Eventos/Evento.cs

using SIG_DefesaCivil.API.Enums;

namespace SIG_DefesaCivil.API.Models.Eventos
{
    public class Evento
    {
        public string Id { get; set; }
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Endereco { get; set; }
        public DateTime DataEHoraDoEvento { get; set; }
        public DateTime DataEntradaNaFaseAtual { get; set; } = DateTime.Now;
        public bool isVisible { get; set; } = true;

        // Relacionamento com usuários
        public string UsuarioCriadorId { get; set; }
        public Usuario UsuarioCriador { get; set; }

        // Auto-relação de hierarquia
        public string? EventoPaiId { get; set; }
        public Evento? EventoPai { get; set; }
        public ICollection<Evento>? SubEventos { get; set; } = new List<Evento>();

        // N-N com eventos
        public ICollection<Natureza> Naturezas { get; set; } = new List<Natureza>();

        // N - 1 com etapas
        public string EtapaId { get; set; }
        public Etapa Etapa { get; set; }
    }
}