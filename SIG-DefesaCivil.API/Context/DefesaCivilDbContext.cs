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

            // === Enum de status ===
            modelBuilder.Entity<Evento>()
                .Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

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

            // === Nomes das tabelas (opcional, para padronizar) ===
            modelBuilder.Entity<Evento>().ToTable("Eventos");
            modelBuilder.Entity<Natureza>().ToTable("Naturezas");
            modelBuilder.Entity<EventoHistorico>().ToTable("EventosHistoricos");
        }
    }
}
