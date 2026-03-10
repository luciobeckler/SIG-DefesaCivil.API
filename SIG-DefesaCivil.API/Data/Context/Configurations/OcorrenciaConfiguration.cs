using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIG_DefesaCivil.API.Helper;
using SIG_DefesaCivil.API.Models.Ocorrencia;

namespace SIG_DefesaCivil.API.Data.Context.Configurations
{
    public class OcorrenciaConfiguration : IEntityTypeConfiguration<Ocorrencia>
    {
        public void Configure(EntityTypeBuilder<Ocorrencia> builder)
        {
            builder.ToTable("Ocorrencias");

            builder.HasMany(e => e.Transicoes)
                    .WithOne(e => e.Ocorrencia)
                    .HasForeignKey(e => e.OcorrenciaId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Responsavel)
                   .WithMany(u => u.OcorrenciasCriados) // Assumindo que a propriedade no Usuario seja OcorrenciasCriados
                   .HasForeignKey(e => e.ResponsavelId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Etapa)
                   .WithMany(s => s.Ocorrencias)
                   .HasForeignKey(e => e.EtapaId)
                   .OnDelete(DeleteBehavior.Restrict);

            // --- 2. Mapeamento do Value Object (Campos) ---
            builder.OwnsOne(e => e.Campos, campos =>
            {
                // Single Selects 
                campos.Property(c => c.GrauDeRisco).HasConversion<string>();
                campos.Property(c => c.RegimeDeOcupacaoDoImovel).HasConversion<string>();

                // Listas de Enums
                campos.ConfigureEnumList(c => c.AnalisePreliminar);
                campos.ConfigureEnumList(c => c.CaracterizacaoDoLocal);
                campos.ConfigureEnumList(c => c.Edificacao);
                campos.ConfigureEnumList(c => c.Estrutura);
                campos.ConfigureEnumList(c => c.TipoDeRisco);
                campos.ConfigureEnumList(c => c.TipificacaoDaOcorrencia);
                campos.ConfigureEnumList(c => c.Motivacao);
                campos.ConfigureEnumList(c => c.AreasAfetadas);
            });
        }
    }
}