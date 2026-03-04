using SIG_DefesaCivil.API.Data.Models.Shared;
using System.Text.Json.Serialization;

namespace SIG_DefesaCivil.API.Models
{
    public class Anexo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string NomeOriginal { get; set; }
        public string UrlArmazenamento { get; set; }
        public string IdArquivoExterno { get; set; }
        public string TipoConteudo { get; set; }
        public long TamanhoBytes { get; set; }
        public DateTime DataUpload { get; set; } = DateTime.UtcNow;
        public string EntidadeId { get; set; }
        public ETiposEntidades TipoEntidade { get; set; }

        public Endereco? Localizacao { get; set; }
        public DateTime? DataHoraCaptura { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ETiposEntidades
    {
        Ocorrencia
    }
}