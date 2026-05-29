using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class ComprasDbContext : DbContext
{
    public ComprasDbContext(DbContextOptions<ComprasDbContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Categoria
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nome).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Icone).HasMaxLength(20);
            entity.Property(c => c.CorHex).HasMaxLength(7);

            entity.HasOne(c => c.Usuario)
                  .WithMany(u => u.Categorias)
                  .HasForeignKey(c => c.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade Compra
        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Descricao).IsRequired().HasMaxLength(150);
            entity.Property(c => c.Valor).HasConversion<double>(); // SQLite armazena decimal melhor como double/real
            entity.Property(c => c.Data).IsRequired();

            // Configuração do relacionamento 1:N
            entity.HasOne(c => c.Categoria)
                  .WithMany(cat => cat.Compras)
                  .HasForeignKey(c => c.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict); // Prevenir deletar categoria se houver compras associadas

            entity.HasOne(c => c.Usuario)
                  .WithMany(u => u.Compras)
                  .HasForeignKey(c => c.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.SenhaHash).IsRequired();
            entity.Property(u => u.Nome).IsRequired().HasMaxLength(100);
        });
    }
}
