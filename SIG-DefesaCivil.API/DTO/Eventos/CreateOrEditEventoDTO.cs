using SIG_DefesaCivil.API.Enums;

namespace SIG_DefesaCivil.API.DTO.Eventos
{
    public class CreateOrEditEventoDTO
    {
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Endereco { get; set; }
        public string Status { get; set; }
        public string? EventoPaiId { get; set; }
        public ICollection<string>? SubEventosId{ get; set; }
        public DateTime DataEHoraDoEvento { get; set; }
    }
}
