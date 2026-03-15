using IMS.CoreBusiness;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class IMSDBContext : DbContext
    {
        public IMSDBContext(DbContextOptions<IMSDBContext> options) : base(options)
        {
            
        }
        public DbSet<Inventory>? Inventories { get; set; }

        public DbSet<Product>? Products { get; set; }

        public DbSet<ProductInventory>? ProductInventories { get; set; }

        public DbSet<InventoryTransaction>? InventoryTransactions { get; set; }

        public DbSet<ProductTransaction>? ProductTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductInventory>()
                .HasKey(pi => new { pi.ProductID, pi.InventoryID });

            modelBuilder.Entity<ProductInventory>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductInventories)
                .HasForeignKey(pi => pi.ProductID);

            modelBuilder.Entity<ProductInventory>()
                .HasOne(pi => pi.Inventory)
                .WithMany(i => i.ProductInventories)
                .HasForeignKey(pi => pi.InventoryID);

            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductId);

            modelBuilder.Entity<Inventory>()
                .HasKey(i => i.InventoryId);

            modelBuilder.Entity<InventoryTransaction>()
                .HasKey(it => it.InventoryTransactionId);

            modelBuilder.Entity<ProductTransaction>()
                .HasKey(pt => pt.ProductTransactionId);

            // Product to ProductTransaction (One-to-Many)
            modelBuilder.Entity<ProductTransaction>()
                .HasOne(pt => pt.Product)
                .WithMany()
                .HasForeignKey(pt => pt.ProductId);

            // Inventory to InventoryTransaction (One-to-Many)
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Inventory)
                .WithMany()
                .HasForeignKey(it => it.InventoryId);

            // Seed Data for Inventories
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { InventoryId = 1, InventoryName = "Bike Seat", Quantity = 10, Price = 2 },
                new Inventory { InventoryId = 2, InventoryName = "Bike Body", Quantity = 10, Price = 15 },
                new Inventory { InventoryId = 3, InventoryName = "Bike Wheels", Quantity = 20, Price = 8 },
                new Inventory { InventoryId = 4, InventoryName = "Bike Pedals", Quantity = 20, Price = 4 }
            );

            // Seed Data for Products
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, ProductName = "Bike", Quantity = 10, Price = 150 },
                new Product { ProductId = 2, ProductName = "Car", Quantity = 5, Price = 25000 }
            );

            // Seed Data for ProductInventory (Many-to-Many relationship)
            modelBuilder.Entity<ProductInventory>().HasData(
                new ProductInventory { ProductID = 1, InventoryID = 1, InventoryQuantity = 1 },
                new ProductInventory { ProductID = 1, InventoryID = 2, InventoryQuantity = 1 },
                new ProductInventory { ProductID = 1, InventoryID = 3, InventoryQuantity = 2 },
                new ProductInventory { ProductID = 1, InventoryID = 4, InventoryQuantity = 2 }
            );
        }
    }
}
