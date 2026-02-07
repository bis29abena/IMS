using IMS.CoreBusiness;

namespace IMS.UseCases.Inventories.Interface
{
    public interface IViewInventoryByIdUseCase
    {
        Task<Inventory?> ExecuteAsync(int inventoryId);
    }
}