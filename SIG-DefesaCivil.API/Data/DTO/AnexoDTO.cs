using SIG_DefesaCivil.API.Data.Models.Shared;
using SIG_DefesaCivil.API.Models;

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
        public Endereco? Localizacao { get; set; }
        public DateTime? DataHoraCaptura { get; set; }
    }

    // 2. DTO de Entrada para Upload (Input)
    public class ListaDeAnexosDTO
    {
        public ETiposEntidades TipoEntidade { get; set; }
        public List<AnexoUploadDTO> Anexos { get; set; } = new List<AnexoUploadDTO>();
    }

    // 3. Objeto complexo filho
    public class AnexoUploadDTO
    {
        public IFormFile Anexo { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public DateTime? DataHoraCaptura { get; set; }
    }

    // 4. DTO para Remoção em Lote
    public class RemocaoAnexosDTO
    {
        public List<string> IdsAnexos { get; set; } = new List<string>();
    }
}