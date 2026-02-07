using IMS.CoreBusiness;

namespace IMS.UseCases.Inventories.Interface
{
    public interface IUpdateInventoryUseCase
    {
        Task ExecuteAsync(Inventory inventory);
    }
}