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
    public class SearchProductTransactionUseCase : ISearchProductTransactionUseCase
    {
        private readonly IProductionTransactionRepository productionTransactionRepository;

        public SearchProductTransactionUseCase(IProductionTransactionRepository productionTransactionRepository)
        {
            this.productionTransactionRepository = productionTransactionRepository;
        }
        public async Task<IEnumerable<ProductTransaction>> ExecuteAync(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? productTransactionType)
        {
            if (dateTo.HasValue) dateTo = dateTo.Value.AddDays(1);

            return await this.productionTransactionRepository.GetProductTransactions(productName, dateFrom, dateTo, productTransactionType);
        }
    }
}
