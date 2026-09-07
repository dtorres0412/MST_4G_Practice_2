using Microsoft.EntityFrameworkCore;
using MST_4G.Models;

namespace MST_4G.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<County> County { get; set; }
    public DbSet<Zo> Zo { get; set; }
    public DbSet<DistrictOffice> DistrictOffice { get; set; }
    public DbSet<Zip> Zip { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Zip>().ToTable("zip");
        modelBuilder.Entity<Zip>().HasKey(z => z.ZipNo);
        modelBuilder.Entity<Zip>().Property(z => z.ZipNo).HasColumnName("zip_no");
        modelBuilder.Entity<Zip>().Property(z => z.ZipName).HasColumnName("zip_name");
        modelBuilder.Entity<Zip>().Property(z => z.EffDateFrom).HasColumnName("eff_date_from");
        modelBuilder.Entity<Zip>().Property(z => z.EffDateTo).HasColumnName("eff_date_to");
        modelBuilder.Entity<Zip>().Property(z => z.CountyNo).HasColumnName("county_no");
        modelBuilder.Entity<Zip>().Property(z => z.ZoNo).HasColumnName("zo_no");
        modelBuilder.Entity<Zip>().Property(z => z.DoNo).HasColumnName("do_no");

        modelBuilder.Entity<County>().ToTable("county");
        modelBuilder.Entity<County>().HasKey(c => c.CountyNo);
        modelBuilder.Entity<County>().Property(c => c.CountyNo).HasColumnName("county_no");
        modelBuilder.Entity<County>().Property(c => c.CountyName).HasColumnName("county_name");

        modelBuilder.Entity<Zo>().ToTable("zo");
        modelBuilder.Entity<Zo>().HasKey(z => z.ZoNo);
        modelBuilder.Entity<Zo>().Property(z => z.ZoNo).HasColumnName("zo_no");
        modelBuilder.Entity<Zo>().Property(z => z.ZoName).HasColumnName("zo_name");

        modelBuilder.Entity<DistrictOffice>().ToTable("district_office");
        modelBuilder.Entity<DistrictOffice>().HasKey(d => d.DoNo);
        modelBuilder.Entity<DistrictOffice>().Property(d => d.DoNo).HasColumnName("do_no");
        modelBuilder.Entity<DistrictOffice>().Property(d => d.DoName).HasColumnName("do_name");

        modelBuilder.Entity<County>().HasKey(c => c.CountyNo);
        modelBuilder.Entity<Zo>().HasKey(z => z.ZoNo);
        modelBuilder.Entity<DistrictOffice>().HasKey(d => d.DoNo);
        modelBuilder.Entity<Zip>().HasKey(z => z.ZipNo);

        modelBuilder.Entity<Zip>()
            .HasOne(z => z.County)
            .WithMany()
            .HasForeignKey(z => z.CountyNo);

        modelBuilder.Entity<Zip>()
            .HasOne(z => z.Zo)
            .WithMany()
            .HasForeignKey(z => z.ZoNo);

        modelBuilder.Entity<Zip>()
            .HasOne(z => z.DistrictOffice)
            .WithMany()
            .HasForeignKey(z => z.DoNo);
    }
}