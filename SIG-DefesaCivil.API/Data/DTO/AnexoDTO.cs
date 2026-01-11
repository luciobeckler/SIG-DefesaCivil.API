namespace SIG_DefesaCivil.API.Data.DTO
{
    public class AnexoDTO
    {
        public string Id { get; set; }
        public string NomeOriginal { get; set; }
        public string UrlArmazenamento { get; set; }
        public string TipoConteudo { get; set; }
        public long TamanhoBytes { get; set; }
    }
}
