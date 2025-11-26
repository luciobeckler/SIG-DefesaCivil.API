namespace SIG_DefesaCivil.API.Models
{
    public class Quadro
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Nome { get; set; }
        public string Descricao { get; set; }

        // Um Quadro tem várias Etapas
        public ICollection<Etapa> Etapas{ get; set; }
    }
}
