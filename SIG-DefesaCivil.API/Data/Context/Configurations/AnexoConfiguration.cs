using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Data.Context.Configurations
{
    public class AnexoConfiguration : IEntityTypeConfiguration<Anexo>
    {
        public void Configure(EntityTypeBuilder<Anexo> builder)
        {
            builder.ToTable("Anexos");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.UrlArmazenamento).IsRequired();
            builder.Property(a => a.IdAnexoExterno).IsRequired();
            builder.Property(a => a.TipoEntidade).IsRequired();
        }
    }
}
