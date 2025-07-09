using Microsoft.EntityFrameworkCore;
using Stock.Entity.Entities;

namespace Stock.Entity
{
    public class AdventureWorksDbContext : DbContext
    {
        public AdventureWorksDbContext(DbContextOptions<AdventureWorksDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductInventory> ProductInventories { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public DbSet<SalesOrderHeader> SalesOrderHeaders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SalesOrderHeader>()
                .ToTable("SalesOrderHeader", "Sales")
                .HasKey(s => s.SalesOrderId);

            modelBuilder.Entity<SalesOrderDetail>()
                .ToTable("SalesOrderDetail", "Sales")
                .HasOne(s => s.SalesOrderHeader)
                .WithMany(soh => soh.SalesOrderDetails)
                .HasForeignKey(s => s.SalesOrderId);

            modelBuilder.Entity<SalesOrderDetail>()
                .ToTable("SalesOrderDetail", "Sales")
                .HasKey(s => new { s.SalesOrderId, s.SalesOrderDetailId });

            modelBuilder.Entity<ProductInventory>()
                .ToTable("ProductInventory", "Production")
                .HasKey(p => new { p.ProductId, p.LocationId });

            modelBuilder.Entity<Product>()
                .ToTable("Product", "Production")
                .HasKey(p => p.ProductId);
        }
    }
}
