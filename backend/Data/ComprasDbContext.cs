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
    public DbSet<DespesaFixa> DespesasFixas => Set<DespesaFixa>();
    public DbSet<PagamentoDespesaFixa> PagamentosDespesasFixas => Set<PagamentoDespesaFixa>();

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

        // Configuração da entidade DespesaFixa
        modelBuilder.Entity<DespesaFixa>(entity =>
        {
            entity.HasKey(df => df.Id);
            entity.Property(df => df.Descricao).IsRequired().HasMaxLength(150);
            entity.Property(df => df.Valor).HasConversion<double>();
            entity.Property(df => df.DiaVencimento).IsRequired();
            entity.Property(df => df.Ativa).HasDefaultValue(true);
            entity.Property(df => df.DataCriacao).IsRequired();

            entity.HasOne(df => df.Categoria)
                  .WithMany()
                  .HasForeignKey(df => df.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(df => df.Usuario)
                  .WithMany()
                  .HasForeignKey(df => df.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade PagamentoDespesaFixa
        modelBuilder.Entity<PagamentoDespesaFixa>(entity =>
        {
            entity.HasKey(pdf => pdf.Id);
            entity.Property(pdf => pdf.Mes).IsRequired();
            entity.Property(pdf => pdf.Ano).IsRequired();
            entity.Property(pdf => pdf.Pago).IsRequired();
            entity.Property(pdf => pdf.ValorPago).HasConversion<double>();

            entity.HasOne(pdf => pdf.DespesaFixa)
                  .WithMany(df => df.Pagamentos)
                  .HasForeignKey(pdf => pdf.DespesaFixaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pdf => pdf.CompraGerada)
                  .WithOne(c => c.PagamentoDespesaFixa)
                  .HasForeignKey<Compra>(c => c.PagamentoDespesaFixaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

}
