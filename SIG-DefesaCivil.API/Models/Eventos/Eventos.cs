namespace SIG_DefesaCivil.API.Models.Eventos
{
    public class Evento
    {
        public string Id { get; set; }
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Endereco { get; set; }
        public string Status { get; set; }
        public DateTime DataEHoraDoEvento { get; set; }

        // Relacionamento com usuários: Um usuário pode ser responsável por vários eventos
        public string UsuarioCriadorId { get; set; }
        public Usuario UsuarioCriador { get; set; }

        // Auto-relação: Um evento pode ter subeventos
        public string? EventoPaiId { get; set; }
        public Evento? EventoPai { get; set; }
        public ICollection<Evento>? SubEventos { get; set; }
    }
}
