namespace SIG_DefesaCivil.API.DTO.Ocorrencias
{
    public class HistoricoOcorrenciaDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EventoId { get; set; }
        public string UsuarioId { get; set; }
        public string Acao { get; set; } // "Visualizou detalhes" ou "Editou de ocorrencia"
        public DateTime UltimaAlteracao { get; set; } = DateTime.UtcNow;
    }
}
