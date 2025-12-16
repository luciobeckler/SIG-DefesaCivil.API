namespace SIG_DefesaCivil.API.Models
{
    public class OcorrenciaHistorico
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OcorrenciaId { get; set; }
        public Ocorrencia.Ocorrencia Ocorrencia { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public string Acao { get; set; } // "Visualizou detalhes" ou "Editou de ocorrencia"
        public DateTime UltimaAlteracao { get; set; } = DateTime.UtcNow;
    }
}
