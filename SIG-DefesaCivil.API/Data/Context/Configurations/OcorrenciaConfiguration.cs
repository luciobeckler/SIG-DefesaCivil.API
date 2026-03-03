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

            // Relacionamentos
            builder.HasMany(e => e.SubOcorrencias)
                   .WithOne(e => e.OcorrenciaPai)
                   .HasForeignKey(e => e.OcorrenciaPaiId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Naturezas)
                   .WithMany(n => n.Ocorrencias)
                   .UsingEntity(j => j.ToTable("OcorrenciaNaturezas"));

            builder.HasOne(e => e.UsuarioCriador)
                   .WithMany(u => u.OcorrenciasCriados)
                   .HasForeignKey(e => e.UsuarioCriadorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Etapa)
                   .WithMany(s => s.Ocorrencias)
                   .HasForeignKey(e => e.EtapaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ConfigureEnumList(e => e.Campos.AnalisePreliminar);
            builder.ConfigureEnumList(e => e.Campos.CaracterizacaoDoLocal);
            builder.ConfigureEnumList(e => e.Campos.Edificacao);
            builder.ConfigureEnumList(e => e.Campos.Estrutura);
            builder.ConfigureEnumList(e => e.Campos.TipoDeRisco);
            builder.ConfigureEnumList(e => e.Campos.TipificacaoDaOcorrencia);
            builder.ConfigureEnumList(e => e.Campos.Motivacao);
            builder.ConfigureEnumList(e => e.Campos.AreasAfetadas);

            // Single Selects (salvar como string)
            builder.Property(e => e.Campos.GrauDeRisco).HasConversion<string>();
            builder.Property(e => e.Campos.RegimeDeOcupacaoDoImovel).HasConversion<string>();
        }
    }
}