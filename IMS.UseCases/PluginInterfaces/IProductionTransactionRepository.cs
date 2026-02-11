using IMS.CoreBusiness;
using IMS.CoreBusiness.Enums;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IProductionTransactionRepository
    {
        Task<IEnumerable<ProductTransaction>> GetProductTransactions(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? productTransactionType);
        Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy, double? price);
        Task SellProductAsync(string salesOrderNumber, Product product, int quantity,double price, string doneBy);
    }
}