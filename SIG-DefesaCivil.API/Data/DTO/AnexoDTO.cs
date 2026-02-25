namespace SIG_DefesaCivil.API.Data.DTO
{
    // 1. DTO de Saída (Output)
    public class AnexoDTO
    {
        public string Id { get; set; }
        public string NomeOriginal { get; set; }
        public string UrlArmazenamento { get; set; }
        public string TipoConteudo { get; set; }
        public long TamanhoBytes { get; set; }
        public string? LatitudeCaptura { get; set; }
        public string? LongitudeCaptura { get; set; }
        public DateTime? DataHoraCaptura { get; set; }
    }

    // 2. DTO de Entrada para Upload (Input)
    public class UploadAnexosDTO
    {
        public string Entidade { get; set; }
        public List<ArquivoUploadDTO> Arquivos { get; set; } = new List<ArquivoUploadDTO>();
    }

    // 3. Objeto complexo filho
    public class ArquivoUploadDTO
    {
        public IFormFile Arquivo { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public DateTime? DataHoraCaptura { get; set; }
    }

    // 4. DTO para Remoção em Lote
    public class RemocaoAnexosDTO
    {
        public string EntidadeTipo { get; set; }
        public List<string> IdsAnexos { get; set; } = new List<string>();
    }
}