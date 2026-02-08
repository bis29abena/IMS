using IMS.CoreBusiness;
using IMS.UseCases.Activities.interfaces;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Activities
{
    public class SellProductUseCase : ISellProductUseCase
    {
        private readonly IProductionTransactionRepository productionTransactionRepository;
        private readonly IProductRepository productRepository;

        public SellProductUseCase(IProductionTransactionRepository productionTransactionRepository, IProductRepository productRepository)
        {
            this.productionTransactionRepository = productionTransactionRepository;
            this.productRepository = productRepository;
        }
        public async Task ExecuteAsync(string salesOrderNumber, Product product, int quantity, double price, string doneBy)
        {
            await this.productionTransactionRepository.SellProductAsync(salesOrderNumber, product, quantity, price, doneBy);

            product.Quantity -= quantity;

            await this.productRepository.UpdateProduct(product);
        }
    }
}
