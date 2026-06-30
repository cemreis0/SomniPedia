using Microsoft.EntityFrameworkCore;
using SomniPedia.Core.Entities;
using MongoDB.EntityFrameworkCore.Extensions;

namespace SomniPedia.Repository;

public class SomniPediaDbContext : DbContext
{
    public DbSet<Article> Articles { get; set; }

    public SomniPediaDbContext(DbContextOptions<SomniPediaDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Article>().ToCollection("articles");
    }
}
