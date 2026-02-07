using IMS.CoreBusiness;

namespace IMS.UseCases.Inventories.Interface
{
    public interface IAddInventoryUseCase
    {
        Task ExecuteAsync(Inventory inventory);
    }
}