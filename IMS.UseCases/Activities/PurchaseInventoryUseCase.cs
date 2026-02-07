using IMS.CoreBusiness;
using IMS.UseCases.Activities.interfaces;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Activities
{
    public class PurchaseInventoryUseCase : IPurchaseInventoryUseCase
    {
        private readonly IInventoryTransactionRespository inventoryTransactionRespository;
        private readonly IInventoryRepository inventoryRepository;

        public PurchaseInventoryUseCase(IInventoryTransactionRespository inventoryTransactionRespository,
            IInventoryRepository inventoryRepository)
        {
            this.inventoryTransactionRespository = inventoryTransactionRespository;
            this.inventoryRepository = inventoryRepository;
        }

        public IInventoryRepository InventoryRepository { get; }

        public async Task ExecuteAsync(string poNumber, Inventory inventory, int quantity, string donrBy)
        {
            //insert a record in the transaction table
            this.inventoryTransactionRespository.PurchaseAsync(poNumber, inventory, quantity, donrBy, inventory.Price);

            inventory.Quantity += quantity;

            await this.inventoryRepository.UpdateInventory(inventory);
        }
    }
}
