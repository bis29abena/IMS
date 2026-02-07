using IMS.CoreBusiness;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IInventoryRepository
    {
        Task AddInventoryAsync(Inventory inventory);
        Task DeleteInventoryAsync(int inventoryId);
        Task<Inventory?> FindInventoryAsync(int inventoryId);
        Task<IEnumerable<Inventory>> GetInvenoriesByNameAsync(string name);
        Task UpdateInventory(Inventory inventory);
    }
}
