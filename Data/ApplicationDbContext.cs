using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Jogo> Jogos => Set<Jogo>();
    public DbSet<BannerAfiliado> BannersAfiliados => Set<BannerAfiliado>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Post>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        builder.Entity<Post>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nome = "Últimas Notícias" },
            new Categoria { Id = 2, Nome = "Mercado da Bola" },
            new Categoria { Id = 3, Nome = "Análise & Opinião" },
            new Categoria { Id = 4, Nome = "DNA Formador" }
        );
    }
}

