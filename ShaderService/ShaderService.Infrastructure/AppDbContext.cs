using Microsoft.EntityFrameworkCore;
using ShaderService.Domain.Entities;

namespace ShaderService.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<ShaderGeneration> ShaderGenerations => Set<ShaderGeneration>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShaderGeneration>(entity =>
        {
            entity.HasKey(u => u.Id);
        });
    }
}