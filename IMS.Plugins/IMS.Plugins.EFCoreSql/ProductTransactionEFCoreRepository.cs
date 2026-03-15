using IMS.CoreBusiness;
using IMS.CoreBusiness.Enums;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class ProductTransactionEFCoreRepository : IProductionTransactionRepository
    {
        private readonly IDbContextFactory<IMSDBContext> dbContextFactory;
        private readonly IProductRepository productRepository;
        private readonly IInventoryTransactionRespository inventoryTransactionRepository;
        private readonly IInventoryRepository inventoryRepository;

        public ProductTransactionEFCoreRepository(
            IDbContextFactory<IMSDBContext> dbContextFactory,
            IProductRepository productRepository,
            IInventoryTransactionRespository inventoryTransactionRepository,
            IInventoryRepository inventoryRepository)
        {
            this.dbContextFactory = dbContextFactory;
            this.productRepository = productRepository;
            this.inventoryTransactionRepository = inventoryTransactionRepository;
            this.inventoryRepository = inventoryRepository;
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy, double? price)
        {
            var prod = await this.productRepository.FindProductAsync(product.ProductId);

            if (prod is not null)
            {
                foreach (var prodInv in prod.ProductInventories)
                {
                    if (prodInv.Inventory is not null)
                    {
                        // Record inventory transaction
                        await this.inventoryTransactionRepository.ProduceAsync(
                            productionNumber,
                            prodInv.Inventory,
                            prodInv.InventoryQuantity * quantity,
                            doneBy,
                            -1);

                        // Decrease inventory quantity
                        var inv = await this.inventoryRepository.FindInventoryAsync(prodInv.InventoryID);
                        if (inv is not null)
                        {
                            inv.Quantity -= prodInv.InventoryQuantity * quantity;
                            await this.inventoryRepository.UpdateInventory(inv);
                        }
                    }
                }
            }

            // Record product transaction
            using var dbContext = this.dbContextFactory.CreateDbContext();

            await dbContext.ProductTransactions!.AddAsync(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                QuantityAfter = product.Quantity + quantity,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task SellProductAsync(string salesOrderNumber, Product product, int quantity, double price, string doneBy)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            await dbContext.ProductTransactions!.AddAsync(new ProductTransaction
            {
                SONumber = salesOrderNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.SellProduct,
                QuantityAfter = product.Quantity - quantity,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
                UnitPrice = price
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactions(
            string productName,
            DateTime? dateFrom,
            DateTime? dateTo,
            ProductTransactionType? productTransactionType)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            var query = dbContext.ProductTransactions!
                .Include(pt => pt.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(productName))
            {
                query = query.Where(pt => pt.Product != null &&
                    EF.Functions.Like(pt.Product.ProductName, $"%{productName}%"));
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(pt => pt.TransactionDate >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(pt => pt.TransactionDate <= dateTo.Value.Date);
            }

            if (productTransactionType.HasValue)
            {
                query = query.Where(pt => pt.ActivityType == productTransactionType);
            }

            return await query.ToListAsync();
        }
    }
}
