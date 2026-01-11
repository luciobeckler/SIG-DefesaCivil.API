using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Ocorrencia;
using System.Linq.Expressions;

namespace SIG_DefesaCivil.API.Data.Context
{
    public class DefesaCivilDbContext : IdentityDbContext<Usuario, IdentityRole, string>
    {
        public DefesaCivilDbContext(DbContextOptions<DefesaCivilDbContext> options)
            : base(options) { }

        // Tabelas
        public DbSet<Natureza> Naturezas { get; set; }
        public DbSet<Ocorrencia> Ocorrencia { get; set; }
        public DbSet<OcorrenciaHistorico> OcorrenciasHistoricos { get; set; }
        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<Quadro> Quadros { get; set; }
        public DbSet<Etapa> Etapas { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Configuração de Naturezas ===
            modelBuilder.Entity<Natureza>()
                .HasMany(n => n.SubNaturezas)
                .WithOne(n => n.NaturezaPai)
                .HasForeignKey(n => n.NaturezaPaiId)
                .OnDelete(DeleteBehavior.Restrict);

            // === Configuração de Ocorrencia (Ocorrencia) ===
            modelBuilder.Entity<Ocorrencia>(entity =>
            {
                entity.ToTable("Ocorrencias");

                entity.HasMany(e => e.SubOcorrencias).WithOne(e => e.OcorrenciaPai).HasForeignKey(e => e.OcorrenciaPaiId).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.Naturezas).WithMany(n => n.Ocorrencias).UsingEntity(j => j.ToTable("OcorrenciaNaturezas"));
                entity.HasOne(e => e.UsuarioCriador).WithMany(u => u.OcorrenciasCriados).HasForeignKey(e => e.UsuarioCriadorId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Etapa).WithMany(s => s.Ocorrencias).HasForeignKey(e => e.EtapaId).OnDelete(DeleteBehavior.Restrict);

                // --- CONFIGURAÇÃO AVANÇADA DE MULTI-SELECT (Com Comparadores) ---
                // Especificando tipos explicitamente <Ocorrencia, TEnum> para evitar erro de inferência

                ConfigureEnumList(entity, e => e.AnalisePreliminar);
                ConfigureEnumList(entity, e => e.CaracterizacaoDoLocal);
                ConfigureEnumList(entity, e => e.Edificacao);
                ConfigureEnumList(entity, e => e.Estrutura);
                ConfigureEnumList(entity, e => e.TipoDeRisco);
                ConfigureEnumList(entity, e => e.TipificacaoDaOcorrencia);
                ConfigureEnumList(entity, e => e.Motivacao);
                ConfigureEnumList(entity, e => e.AreasAfetadas);

                // Single Selects (salvar como string)
                entity.Property(e => e.GrauDeRisco).HasConversion<string>();
                entity.Property(e => e.RegimeDeOcupacaoDoImovel).HasConversion<string>();
            });

            // === Configuração de OcorrenciaHistorico ===
            modelBuilder.Entity<OcorrenciaHistorico>(entity =>
            {
                entity.ToTable("OcorrenciasHistoricos");
                entity.HasOne(h => h.Ocorrencia)
                      .WithMany()
                      .HasForeignKey(h => h.OcorrenciaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.Usuario)
                      .WithMany()
                      .HasForeignKey(h => h.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === Configuração de Anexo ===
            modelBuilder.Entity<Anexo>(entity =>
            {
                entity.ToTable("Anexos");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.NomeOriginal).IsRequired();
                entity.Property(a => a.UrlArmazenamento).IsRequired();
                entity.Property(a => a.IdArquivoExterno).IsRequired();
                entity.Property(a => a.EntidadeId).IsRequired();
                entity.Property(a => a.TipoEntidade).IsRequired();

                entity.HasIndex(a => new { a.EntidadeId, a.TipoEntidade });
            });

            // === Configuração de Quadro (Frame) ===
            modelBuilder.Entity<Quadro>(entity =>
            {
                entity.ToTable("Quadros");
                entity.HasKey(q => q.Id);
                entity.Property(q => q.Nome).IsRequired().HasMaxLength(100);

                entity.HasMany(q => q.Etapas)
                      .WithOne(e => e.Quadro)
                      .HasForeignKey(e => e.QuadroId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === Configuração de Etapa (Stage) ===
            modelBuilder.Entity<Etapa>(entity =>
            {
                entity.ToTable("Etapas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);

                ConfigureEnumList(entity, e => e.PermissoesParaTransicionarParaEstaEtapa);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        /// <summary>
        /// Método auxiliar genérico para configurar listas de Enums (List<T>).
        /// </summary>
        private void ConfigureEnumList<TEntity, TEnum>(
            EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, List<TEnum>>> propertyExpression) // Usa List<T> explicitamente
            where TEntity : class
            where TEnum : struct, Enum
        {
            var comparer = new ValueComparer<List<TEnum>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            builder.Property(propertyExpression)
                .HasConversion(
                    v => string.Join(',', v.Select(e => e.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => Enum.Parse<TEnum>(s)).ToList())
                .Metadata.SetValueComparer(comparer);
        }
    }
}