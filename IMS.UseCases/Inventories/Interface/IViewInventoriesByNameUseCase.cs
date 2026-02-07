using IMS.CoreBusiness;

namespace IMS.UseCases.Inventories.Interface
{
    public interface IViewInventoriesByNameUseCase
    {
        Task<IEnumerable<Inventory>> ExecuteAsync(string name = "");
    }
}