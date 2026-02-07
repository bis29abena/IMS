namespace IMS.UseCases.Inventories.Interface
{
    public interface IDeleteInventoryUseCase
    {
        Task ExecuteAsync(int inventoryId);
    }
}