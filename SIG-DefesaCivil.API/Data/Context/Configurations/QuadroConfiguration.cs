using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Data.Context.Configurations
{
    public class QuadroConfiguration : IEntityTypeConfiguration<Quadro>
    {
        public void Configure(EntityTypeBuilder<Quadro> builder)
        {
            builder.ToTable("Quadros");

            builder.HasKey(q => q.Id);
            builder.Property(q => q.Nome).IsRequired().HasMaxLength(100);

            builder.HasMany(q => q.Etapas)
                   .WithOne(e => e.Quadro)
                   .HasForeignKey(e => e.QuadroId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
