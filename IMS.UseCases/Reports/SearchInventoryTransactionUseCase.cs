using IMS.CoreBusiness;
using IMS.CoreBusiness.Enums;
using IMS.UseCases.PluginInterfaces;
using IMS.UseCases.Reports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Reports
{
    public class SearchInventoryTransactionUseCase : ISearchInventoryTransactionUseCase
    {
        private readonly IInventoryTransactionRespository inventoryTransactionRespository;

        public SearchInventoryTransactionUseCase(IInventoryTransactionRespository inventoryTransactionRespository)
        {
            this.inventoryTransactionRespository = inventoryTransactionRespository;
        }
        public async Task<IEnumerable<InventoryTransaction>> ExecuteAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? inventoryTransactionType)
        {
            if(dateTo.HasValue) dateTo = dateTo.Value.AddDays(1);

            return await this.inventoryTransactionRespository.GetInventories(inventoryName, dateFrom, dateTo, inventoryTransactionType);
        }
    }
}
