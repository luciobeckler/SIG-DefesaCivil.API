namespace SIG_DefesaCivil.API.Data.DTO
{
    public class NaturezaDTO : NaturezaResumoDTO
    {
        public string? NaturezaPaiId { get; set; }

        public List<NaturezaDTO> SubNaturezas { get; set; } = new();
        public bool EhFolha => SubNaturezas == null || !SubNaturezas.Any();
    }

    public class CreateNaturezaDTO
    {
        public string Nome { get; set; }
        public string CodigoNatureza { get; set; }
        public string? Descricao { get; set; }
        public string? CodigoNaturezaPai { get; set; }
    }

    public class NaturezaResumoDTO
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string CodigoNatureza { get; set; }
    }
}
