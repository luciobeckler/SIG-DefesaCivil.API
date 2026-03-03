using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIG_DefesaCivil.API.Models;

namespace SIG_DefesaCivil.API.Data.Context.Configurations
{
    public class OcorrenciaHistoricoConfiguration : IEntityTypeConfiguration<OcorrenciaHistorico>
    {
        public void Configure(EntityTypeBuilder<OcorrenciaHistorico> builder)
        {
            builder.ToTable("OcorrenciasHistoricos");

            builder.HasOne(h => h.Ocorrencia)
                   .WithMany()
                   .HasForeignKey(h => h.OcorrenciaId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.Usuario)
                   .WithMany()
                   .HasForeignKey(h => h.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
