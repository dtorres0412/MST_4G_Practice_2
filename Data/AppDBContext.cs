using Microsoft.EntityFrameworkCore;
using MST_4G.Models;

namespace MST_4G.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<County> County { get; set; } = null!;
    public DbSet<Zo> Zo { get; set; } = null!;
    public DbSet<DistrictOffice> DistrictOffice { get; set; } = null!;
    public DbSet<Zip> Zip { get; set; } = null!;
    public DbSet<ZipJunction> ZipJunction { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Zip>(entity =>
        {
            entity.ToTable("zip");
            entity.HasKey(z => z.ZipId);
            entity.Property(z => z.ZipId).HasColumnName("zip_id");
            entity.Property(z => z.ZipNo).HasColumnName("zip_no");
            entity.Property(z => z.ZipName).HasColumnName("zip_name");
            entity.Property(z => z.EffDateFrom).HasColumnName("eff_date_from");
            entity.Property(z => z.EffDateTo).HasColumnName("eff_date_to");
        });

        modelBuilder.Entity<County>(entity =>
        {
            entity.ToTable("county");
            entity.HasKey(c => c.CountyId);
            entity.Property(c => c.CountyId).HasColumnName("county_id");
            entity.Property(c => c.CountyNo).HasColumnName("county_no");
            entity.Property(c => c.CountyName).HasColumnName("county_name");
        });

        modelBuilder.Entity<Zo>(entity =>
        {
            entity.ToTable("zo");
            entity.HasKey(z => z.ZoId);
            entity.Property(z => z.ZoId).HasColumnName("zo_id");
            entity.Property(z => z.ZoNo).HasColumnName("zo_no");
            entity.Property(z => z.ZoName).HasColumnName("zo_name");
        });

        modelBuilder.Entity<DistrictOffice>(entity =>
        {
            entity.ToTable("district_office");
            entity.HasKey(d => d.DoId);
            entity.Property(d => d.DoId).HasColumnName("do_id");
            entity.Property(d => d.DoNo).HasColumnName("do_no");
            entity.Property(d => d.DoName).HasColumnName("do_name");
        });

        modelBuilder.Entity<ZipJunction>(entity =>
        {
            entity.ToTable("zip_junction");

            entity.HasKey(zj => new { zj.ZipNo, zj.CountyNo, zj.ZoNo, zj.DoNo });

            entity.Property(zj => zj.ZipNo).HasColumnName("zip_no");
            entity.Property(zj => zj.CountyNo).HasColumnName("county_no");
            entity.Property(zj => zj.ZoNo).HasColumnName("zo_no");
            entity.Property(zj => zj.DoNo).HasColumnName("do_no");

            entity.HasOne(zj => zj.Zip)
                  .WithMany(z => z.ZipJunction)
                  .HasForeignKey(zj => zj.ZipNo)
                  .HasPrincipalKey(z => z.ZipNo);

            entity.HasOne(zj => zj.County)
                  .WithMany()
                  .HasForeignKey(zj => zj.CountyNo)
                  .HasPrincipalKey(c => c.CountyNo);

            entity.HasOne(zj => zj.Zo)
                  .WithMany()
                  .HasForeignKey(zj => zj.ZoNo)
                  .HasPrincipalKey(z => z.ZoNo);

            entity.HasOne(zj => zj.DistrictOffice)
                  .WithMany()
                  .HasForeignKey(zj => zj.DoNo)
                  .HasPrincipalKey(d => d.DoNo);
        });
    }
}