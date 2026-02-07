using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IProductionTransactionRepository
    {
        Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy, double? price);
    }
}