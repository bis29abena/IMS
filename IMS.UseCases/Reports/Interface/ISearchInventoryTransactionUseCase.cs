using IMS.CoreBusiness;
using IMS.CoreBusiness.Enums;

namespace IMS.UseCases.Reports.Interface
{
    public interface ISearchInventoryTransactionUseCase
    {
        Task<IEnumerable<InventoryTransaction>> ExecuteAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? inventoryTransactionType);
    }
}