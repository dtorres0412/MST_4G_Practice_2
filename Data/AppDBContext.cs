using Microsoft.EntityFrameworkCore;
using MST_4G.Models;

namespace MST_4G.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Zip> Zip { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set Primary Keys
        modelBuilder.Entity<Zip>().HasKey(z => z.ZipNo);

    }
}