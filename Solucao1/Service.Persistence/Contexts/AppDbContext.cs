using Microsoft.EntityFrameworkCore;
using Service.Entities.Models;

namespace Service.Persistence.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<FaturaModel> Faturas { get; set; } = default!;
    public DbSet<FaturaItemModel> FaturaItems { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}