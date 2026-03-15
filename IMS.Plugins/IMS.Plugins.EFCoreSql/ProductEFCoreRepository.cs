using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class ProductEFCoreRepository : IProductRepository
    {
        private readonly IDbContextFactory<IMSDBContext> dbContextFactory;

        public ProductEFCoreRepository(IDbContextFactory<IMSDBContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task AddProductAsync(Product product)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            dbContext.Products!.Add(product);

            await FlagInventoryUnchanged(product, dbContext);

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int productId)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            await dbContext.Products!
                .Where(p => p.ProductId == productId)
                .ExecuteDeleteAsync();
        }

        public async Task<Product?> FindProductAsync(int productId)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            return await dbContext.Products!
                .Include(p => p.ProductInventories)
                    .ThenInclude(pi => pi.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            if (string.IsNullOrWhiteSpace(name))
                return await dbContext.Products!.ToListAsync();

            return await dbContext.Products!
                .Where(p => EF.Functions.Like(p.ProductName, $"%{name}%"))
                .ToListAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            var productToUpdate = await dbContext.Products!
                .Include(p => p.ProductInventories)
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            if (productToUpdate != null)
            {
                dbContext.Entry(productToUpdate).CurrentValues.SetValues(product);

                // Update ProductInventories (remove old, add new)
                productToUpdate.ProductInventories.Clear();
                foreach (var pi in product.ProductInventories)
                {
                    productToUpdate.ProductInventories.Add(new ProductInventory
                    {
                        ProductID = pi.ProductID,
                        InventoryID = pi.InventoryID,
                        InventoryQuantity = pi.InventoryQuantity
                    });
                }

                await FlagInventoryUnchanged(product, dbContext);

                await dbContext.SaveChangesAsync();
            }
        }

        private async Task FlagInventoryUnchanged(Product product, IMSDBContext db)
        {
            if (product.ProductInventories is not null && product.ProductInventories.Count > 0)
            {
                foreach (var prodInventory in product.ProductInventories)
                {
                    var inventory = await db.Inventories!.FindAsync(prodInventory.InventoryID);
                    if (inventory != null)
                    {
                        db.Entry(inventory).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
