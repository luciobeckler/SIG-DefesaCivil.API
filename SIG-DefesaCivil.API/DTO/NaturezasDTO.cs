namespace SIG_DefesaCivil.API.DTOs
{
    public class NaturezaDto : NaturezaResumoDTO
    {
        public string? NaturezaPaiId { get; set; }

        public List<NaturezaDto> SubNaturezas { get; set; } = new();
    }

    public class CreateNaturezaDto
    {
        public string Nome { get; set; }
        public string CodigoNatureza { get; set; }
        public string? CodigoNaturezaPai { get; set; }
    }

    public class NaturezaResumoDTO
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string CodigoNatureza { get; set; }
    }
}
