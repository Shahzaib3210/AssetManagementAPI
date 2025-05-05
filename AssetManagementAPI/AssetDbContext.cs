using AssetManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementAPI
{
    public class AssetDbContext : DbContext
    {
        public AssetDbContext(DbContextOptions<AssetDbContext> options) : base(options) { }

        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<BalanceEntry> BalanceEntries => Set<BalanceEntry>();
        public DbSet<Holding> Holdings => Set<Holding>();
        public DbSet<AssetInfo> AssetInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AssetInfo>().ToTable("AssetInfo");

            // Configure Asset entity
            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.AssetId).IsRequired();
                entity.HasIndex(a => a.AssetId).IsUnique();

                entity.Property(a => a.IncludeInNetWorth).HasDefaultValue(true);
                entity.Property(a => a.IsActive).HasDefaultValue(true);

                // Configure relationships
                entity.HasMany(a => a.Balances)
                    .WithOne(b => b.Asset)
                    .HasForeignKey(b => b.AssetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(a => a.Holdings)
                    .WithOne(h => h.Asset)
                    .HasForeignKey(h => h.AssetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure BalanceEntry entity
            modelBuilder.Entity<BalanceEntry>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.BalanceAsOf).IsRequired();
                entity.Property(b => b.Amount).IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(b => b.BalanceCostBasis)
                    .HasColumnType("decimal(18,2)");

                entity.Property(b => b.BalanceQuantityCurrent)
                    .HasColumnType("decimal(18,2)");
            });

            // Configure Holding entity
            modelBuilder.Entity<Holding>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Value)
                    .HasColumnType("decimal(18,2)");
            });

            // Configure decimal precision for all decimal properties
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties()))
            {
                if (property.ClrType == typeof(decimal))
                {
                    property.SetPrecision(18);
                    property.SetScale(2);
                }
            }
        }
    }
}
