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

            // --- 1. Relacionamentos da Entidade Pai ---
            builder.HasMany(e => e.SubOcorrencias)
                   .WithOne(e => e.OcorrenciaPai)
                   .HasForeignKey(e => e.OcorrenciaPaiId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Naturezas)
                   .WithMany(n => n.Ocorrencias)
                   .UsingEntity(j => j.ToTable("OcorrenciaNaturezas"));

            builder.HasMany(e => e.Transicoes)
                    .WithOne(e => e.Ocorrencia)
                    .HasForeignKey(e => e.OcorrenciaId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.UsuarioCriador)
                   .WithMany(u => u.OcorrenciasCriados)
                   .HasForeignKey(e => e.UsuarioCriadorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Etapa)
                   .WithMany(s => s.Ocorrencias)
                   .HasForeignKey(e => e.EtapaId)
                   .OnDelete(DeleteBehavior.Restrict);

            // --- 2. Mapeamento do Value Object (Campos) ---
            builder.OwnsOne(e => e.Campos, campos =>
            {
                // Dica: O EF Core vai criar colunas no banco com o prefixo do objeto (ex: "Campos_GrauDeRisco").
                // Se você quiser que a coluna no banco se chame apenas "GrauDeRisco", descomente a linha abaixo para cada campo:
                // campos.Property(c => c.GrauDeRisco).HasColumnName("GrauDeRisco");

                // Single Selects (salvar como string)
                campos.Property(c => c.GrauDeRisco).HasConversion<string>();
                campos.Property(c => c.RegimeDeOcupacaoDoImovel).HasConversion<string>();

                campos.Navigation(c => c.Localizacao).IsRequired();

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