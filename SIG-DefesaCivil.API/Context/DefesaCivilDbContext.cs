using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Eventos;

namespace SIG_DefesaCivil.API.Context
{
    public class DefesaCivilDbContext : IdentityDbContext<Usuario, IdentityRole, string>
    {
        public DefesaCivilDbContext(DbContextOptions<DefesaCivilDbContext> options)
            : base(options){}

        // Tabelas
        public DbSet<Natureza> Natureza { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<EventoHistorico> EventosHistoricos { get; set; }
        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<Quadro> Quadros { get; set; }
        public DbSet<Etapa> Etapas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Configuração de Naturezas ===
            modelBuilder.Entity<Natureza>()
                .HasMany(n => n.SubNaturezas)
                .WithOne(n => n.NaturezaPai)
                .HasForeignKey(n => n.NaturezaPaiId)
                .OnDelete(DeleteBehavior.Restrict);

            // === Auto-relacionamento de Eventos ===
            modelBuilder.Entity<Evento>()
                .HasMany(e => e.SubEventos)
                .WithOne(e => e.EventoPai)
                .HasForeignKey(e => e.EventoPaiId)
                .OnDelete(DeleteBehavior.Restrict);

            /// N-N Eventos e naturezaas
            modelBuilder.Entity<Evento>()
                .HasMany(e => e.Naturezas)     
                .WithMany(n => n.Eventos)       
                .UsingEntity(j => j.ToTable("EventoNaturezas"));

            // === Relacionamento Evento ↔ Usuário ===
            modelBuilder.Entity<Evento>()
                .HasOne(e => e.UsuarioCriador)
                .WithMany(u => u.EventosCriados)
                .HasForeignKey(e => e.UsuarioCriadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // === Relacionamento EventoHistorico ↔ Evento ===
            modelBuilder.Entity<EventoHistorico>()
                .HasOne(h => h.Evento)
                .WithMany()
                .HasForeignKey(h => h.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            // === Relacionamento EventoHistorico ↔ Usuario ===
            modelBuilder.Entity<EventoHistorico>()
                .HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Anexo>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.NomeOriginal).IsRequired();
                entity.Property(a => a.UrlArmazenamento).IsRequired();
                entity.Property(a => a.IdArquivoExterno).IsRequired();
                entity.Property(a => a.EntidadeId).IsRequired();
                entity.Property(a => a.TipoEntidade).IsRequired();

                entity.HasIndex(a => new { a.EntidadeId, a.TipoEntidade });

                // === Frame (Quadro) ===
                modelBuilder.Entity<Etapa>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                    // Relação 1-N: Frame -> Stages
                    entity.HasOne(e => e.Quadro)
                          .WithMany(q => q.Etapas)
                          .HasForeignKey(s => s.QuadroId)
                          .OnDelete(DeleteBehavior.Cascade); // Deletar Quadro -> Deleta Stages
                });

                // === Stage (Etapa) ===
                modelBuilder.Entity<Etapa>(entity =>
                {
                    entity.HasKey(s => s.Id);
                    entity.Property(s => s.Nome).IsRequired().HasMaxLength(100);
                    entity.HasMany(s => s.Eventos)
                          .WithOne(e => e.Etapa)
                          .HasForeignKey(e => e.EtapaId)
                          .OnDelete(DeleteBehavior.Restrict); // Proteção: Não deletar Etapa se tiver Eventos nela
                });
            });

            // === Nomes das tabelas (opcional, para padronizar) ===
            modelBuilder.Entity<Evento>().ToTable("Eventos");
            modelBuilder.Entity<Natureza>().ToTable("Naturezas");
            modelBuilder.Entity<EventoHistorico>().ToTable("EventosHistoricos");
            modelBuilder.Entity<Anexo>().ToTable("Anexos");
            modelBuilder.Entity<Quadro>().ToTable("Quadros");
            modelBuilder.Entity<Etapa>().ToTable("Etapas");


        }
    }
}
