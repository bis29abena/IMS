using IMS.CoreBusiness;
using IMS.CoreBusiness.Enums;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IInventoryTransactionRespository
    {
        void PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price);
        void ProduceAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, double price);
        Task<IEnumerable<InventoryTransaction>> GetInventories(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? inventoryTransactionType);
    }
}