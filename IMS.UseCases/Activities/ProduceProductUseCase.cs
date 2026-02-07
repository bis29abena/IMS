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
    public class ProduceProductUseCase : IProduceProductUseCase
    {
        private readonly IProductionTransactionRepository productionTransactionRepository;
        private readonly IProductRepository productRepository;

        public ProduceProductUseCase(IProductionTransactionRepository productionTransactionRepository, IProductRepository productRepository)
        {
            this.productionTransactionRepository = productionTransactionRepository;
            this.productRepository = productRepository;
        }
        public async Task ExecuteAsync(string productionNumber, Product product, int quantity, string doneBy, double? price)
        {
            // add transaction Record
            await this.productionTransactionRepository.ProduceAsync(productionNumber, product, quantity, doneBy, price);
            // decrease the quantity inventories
            // update the quantity of product
            product.Quantity += quantity;

            await this.productRepository.UpdateProduct(product);
        }
    }
}
