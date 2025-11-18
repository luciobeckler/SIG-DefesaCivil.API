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
        public DbSet<Natureza> Naturezas { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<EventoHistorico> EventosHistoricos { get; set; }
        public DbSet<Models.Anexo> Anexos { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FieldDefinition> FieldsDefinitions { get; set; }
        public DbSet<FormCompleted> FormsCompleted { get; set; }
        public DbSet<FieldResponse> FieldsResponse { get; set; }
        public DbSet<Frame> Frames { get; set; }
        public DbSet<Stage> Stages { get; set; }

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

            modelBuilder.Entity<Models.Anexo>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.NomeOriginal).IsRequired();
                entity.Property(a => a.UrlArmazenamento).IsRequired();
                entity.Property(a => a.IdArquivoExterno).IsRequired();
                entity.Property(a => a.EntidadeId).IsRequired();
                entity.Property(a => a.TipoEntidade).IsRequired();

                entity.HasIndex(a => new { a.EntidadeId, a.TipoEntidade });
            });


            // Frame 1 -> N Stage
            modelBuilder.Entity<Frame>(entity =>
            {
                entity.ToTable("Frames");
                entity.HasMany(f => f.Stages) // Um Frame tem muitos Stages
                      .WithOne(s => s.Frame)  // Um Stage pertence a um Frame
                      .HasForeignKey(s => s.FrameId)
                      .OnDelete(DeleteBehavior.Cascade); // Se deletar o quadro, deleta as colunas
            });

            // Stage 1 -> 1 Form
            modelBuilder.Entity<Stage>(entity =>
            {
                entity.ToTable("Stages");
                entity.HasOne(s => s.Form)
                      .WithMany() // <-- Deixa o Formulario livre para ser usado por muitos
                      .HasForeignKey(s => s.FormId)
                      .OnDelete(DeleteBehavior.SetNull); // Se o molde for deletado, o stage fica sem formulário
            });

            // Formulario 1 -> N CampoDefinicao
            modelBuilder.Entity<Form>()
                .HasMany(f => f.FieldDefinition)
                .WithOne(c => c.Form)
                .HasForeignKey(c => c.FormId)
                .OnDelete(DeleteBehavior.Cascade); // Se deletar o molde, deleta as perguntas

            // Salva o Enum de Tipo de Campo como string
            modelBuilder.Entity<FieldDefinition>()
                .Property(c => c.Type)
                .HasConversion<string>()
                .HasMaxLength(50);

            // FormularioPreenchido 1 -> N RespostaCampo
            modelBuilder.Entity<FormCompleted>()
                .HasMany(fp => fp.Responses)
                .WithOne(r => r.FormCompleted)
                .HasForeignKey(r => r.FormCompletedId)
                .OnDelete(DeleteBehavior.Cascade); // Se deletar o preenchimento, deleta as respostas

            // Relação entre a Resposta e a Pergunta (1-N)
            modelBuilder.Entity<FieldResponse>()
                .HasOne(r => r.FieldDefinition)
                .WithMany() // Uma definição pode ter muitas respostas (em diferentes formulários)
                .HasForeignKey(r => r.FieldDefinitionId)
                .OnDelete(DeleteBehavior.Restrict); // Não deixa deletar uma "pergunta" se ela já foi usada

            // Relação entre o Preenchimento e o Molde (1-N)
            modelBuilder.Entity<FormCompleted>()
                .HasOne(fp => fp.Formulario)
                .WithMany() // Um molde pode ter muitos preenchimentos
                .HasForeignKey(fp => fp.FormId)
                .OnDelete(DeleteBehavior.Restrict); // Não deixa deletar um "molde" se ele já foi usado
        }
    }
}
