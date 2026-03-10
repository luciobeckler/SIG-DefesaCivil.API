using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Data.Context.Configurations
{
    public class NaturezaConfiguration : IEntityTypeConfiguration<Natureza>
    {
        public void Configure(EntityTypeBuilder<Natureza> builder)
        {
            builder.ToTable("Naturezas");

            builder.HasMany(n => n.SubNaturezas)
                   .WithOne(n => n.NaturezaPai)
                   .HasForeignKey(n => n.NaturezaPaiId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
