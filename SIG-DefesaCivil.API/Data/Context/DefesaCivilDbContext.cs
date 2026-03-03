using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Models.Ocorrencia;
using System.Reflection;

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

            AplicarConversaoGlobalUTC(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void AplicarConversaoGlobalUTC(ModelBuilder modelBuilder)
        {
            var utcConverter = new ValueConverter<DateTime, DateTime>(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(utcConverter);
                    }
                }
            }
        }
    }
}