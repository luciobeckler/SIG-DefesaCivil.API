using Microsoft.AspNetCore.Mvc;

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
    public class UploadAnexoDTO
    {
        [FromRoute] public string OcorrenciaId { get; set; }
        [FromForm] public string Entidade { get; set; } // Agora é FromForm
        [FromForm] public List<IFormFile> Arquivos { get; set; }
    }

    // DTO para Remoção em Lote
    public class RemocaoAnexoDto
    {
        public string EntidadeTipo { get; set; }
        public List<string> IdsAnexos { get; set; }
    }
}
