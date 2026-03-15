using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class InventoryEFCoreRepository : IInventoryRepository
    {
        private readonly IDbContextFactory<IMSDBContext> dbContextFactory;

        public InventoryEFCoreRepository(IDbContextFactory<IMSDBContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task AddInventoryAsync(Inventory inventory)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            dbContext.Inventories!.Add(inventory);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteInventoryAsync(int inventoryId)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            await dbContext.Inventories!
                .Where(i => i.InventoryId == inventoryId)
                .ExecuteDeleteAsync();
        }

        public async Task<Inventory?> FindInventoryAsync(int inventoryId)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            return await dbContext.Inventories!.FindAsync(inventoryId);
        }

        public async Task<IEnumerable<Inventory>> GetInvenoriesByNameAsync(string name)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            return await dbContext.Inventories!
                .Where(i => EF.Functions.Like(i.InventoryName, $"%{name}%"))
                .ToListAsync();
        }

        public async Task UpdateInventory(Inventory inventory)
        {
            using var dbContext = this.dbContextFactory.CreateDbContext();

            var inventoryToUpdate = await dbContext.Inventories!.FindAsync(inventory.InventoryId);

            if (inventoryToUpdate != null)
            {
                dbContext.Entry(inventoryToUpdate).CurrentValues.SetValues(inventory);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
