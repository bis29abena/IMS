using IMS.CoreBusiness;
using IMS.CoreBusiness.Enums;

namespace IMS.UseCases.Reports.Interface
{
    public interface ISearchProductTransactionUseCase
    {
        Task<IEnumerable<ProductTransaction>> ExecuteAync(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? productTransactionType);
    }
}