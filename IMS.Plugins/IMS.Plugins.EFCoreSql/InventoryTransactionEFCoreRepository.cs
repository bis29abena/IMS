using IMS.CoreBusiness;
using IMS.CoreBusiness.Enums;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class InventoryTransactionEFCoreRepository : IInventoryTransactionRespository
    {
        private readonly IDbContextFactory<IMSDBContext> dbContextFactory;

        public InventoryTransactionEFCoreRepository(IDbContextFactory<IMSDBContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            await dbContext.InventoryTransactions!.AddAsync(new InventoryTransaction
            {
                PONumber = poNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.PurchaseInvenetory,
                QuantityAfter = inventory.Quantity + quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task ProduceAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, double price)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            await dbContext.InventoryTransactions!.AddAsync(new InventoryTransaction
            {
                ProductionNumber = productionNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.ProduceProduct,
                QuantityAfter = inventory.Quantity - quantityToConsume,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventories(
            string inventoryName,
            DateTime? dateFrom,
            DateTime? dateTo,
            InventoryTransactionType? inventoryTransactionType)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            var query = dbContext.InventoryTransactions!
                .Include(it => it.Inventory)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(inventoryName))
            {
                query = query.Where(it => it.Inventory != null &&
                    EF.Functions.Like(it.Inventory.InventoryName, $"%{inventoryName}%"));
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(it => it.TransactionDate >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(it => it.TransactionDate <= dateTo.Value.Date);
            }

            if (inventoryTransactionType.HasValue)
            {
                query = query.Where(it => it.ActivityType == inventoryTransactionType);
            }

            return await query.ToListAsync();
        }
    }
}
