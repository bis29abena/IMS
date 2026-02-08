using IMS.CoreBusiness;
using IMS.CoreBusiness.Enums;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.InMemory
{
    public class InventoryTransactionRepository : IInventoryTransactionRespository
    {
        private readonly IInventoryRepository inventoryRepository;
        public List<InventoryTransaction> _inventoryTransactions = new List<InventoryTransaction>();

        public InventoryTransactionRepository(IInventoryRepository inventoryRepository)
        {
            this.inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventories(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? inventoryTransactionType)
        {
            // get a list of inventories just to get the inventory name
            var inventories = (await this.inventoryRepository.GetInvenoriesByNameAsync(string.Empty)).ToList();
            // then write your join statement to pass the inventory in there
            var query = from InvTransaction in _inventoryTransactions
                        join inventory in inventories on InvTransaction.InventoryId equals inventory.InventoryId
                        where (string.IsNullOrWhiteSpace(inventoryName) || inventory.InventoryName.ToLower().IndexOf(inventoryName.ToLower()) >= 0) &&
                              (!dateFrom.HasValue || InvTransaction.TransactionDate >= dateFrom.Value.Date) &&
                              (!dateTo.HasValue || InvTransaction.TransactionDate <= dateTo.Value.Date) &&
                              (!inventoryTransactionType.HasValue || InvTransaction.ActivityType == inventoryTransactionType)

                        select new InventoryTransaction
                        {
                            Inventory = inventory,
                            PONumber = InvTransaction.PONumber,
                            InventoryTransactionId = InvTransaction.InventoryTransactionId,
                            QuantityBefore = InvTransaction.QuantityBefore,
                            QuantityAfter = InvTransaction.QuantityAfter,
                            UnitPrice = InvTransaction.UnitPrice,
                            DoneBy = InvTransaction.DoneBy,
                            TransactionDate = InvTransaction.TransactionDate,
                            ActivityType = InvTransaction.ActivityType,
                        };

            return query;
                        
        }

        public void ProduceAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, double price)
        {
            this._inventoryTransactions.Add(new InventoryTransaction
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
        }

        public void PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            this._inventoryTransactions.Add(new InventoryTransaction
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
        }
    }
}
