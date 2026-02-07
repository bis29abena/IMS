using IMS.CoreBusiness;
using IMS.UseCases.Products.Interfaces;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Products
{
    public class UpdateProductUseCase : IUpdateProductUseCase
    {
        private readonly IProductRepository productRepository;

        public UpdateProductUseCase(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public async Task ExecuteAsync(Product product)
        {
            await productRepository.UpdateProduct(product);
        }
    }
}
