namespace SIG_DefesaCivil.API.Data.DTO.Ocorrencia
{
    public class HistoricoOcorrenciaDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OcorrenciaId { get; set; }
        public string UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public string Acao { get; set; } // "Visualizou detalhes" ou "Editou de ocorrencia"
        public List<DateTime> Horarios { get; set; } = new List<DateTime>();
    }
}
