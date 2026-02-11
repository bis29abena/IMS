using IMS.CoreBusiness;
using IMS.CoreBusiness.Enums;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.InMemory
{
    public class ProductTransactionRepository : IProductionTransactionRepository
    {
        private List<ProductTransaction> _productTransactions = new List<ProductTransaction>();

        private readonly IProductRepository productRepository;
        private readonly IInventoryTransactionRespository inventoryTransactionRespository;
        private readonly IInventoryRepository inventoryRepository;

        public ProductTransactionRepository(IProductRepository productRepository, 
            IInventoryTransactionRespository inventoryTransactionRespository,
            IInventoryRepository inventoryRepository
            )
        {
            this.productRepository = productRepository;
            this.inventoryTransactionRespository = inventoryTransactionRespository;
            this.inventoryRepository = inventoryRepository;
        }
        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy, double? price)
        {
            // decrease the inventories
            var prod = await this.productRepository.FindProductAsync(product.ProductId);

            if (prod is not null) 
            {
                foreach (var prodInv in prod.ProductInventories) 
                {
                    if(prodInv is not null)
                    {
                        //adding inventory transactions
                        this.inventoryTransactionRespository.ProduceAsync(productionNumber, prodInv.Inventory, prodInv.InventoryQuantity * quantity,
                            doneBy, -1);

                        var inv = await this.inventoryRepository.FindInventoryAsync(prodInv.InventoryID);

                        if (inv is not null)
                        {
                            // decreasing the inventories
                            inv.Quantity -= prodInv.InventoryQuantity * quantity;
                            await this.inventoryRepository.UpdateInventory(inv);
                        }
                    }
                }
            }
            // add production transaction
            this._productTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                QuantityAfter = product.Quantity + quantity,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy

            });
        }

        public Task SellProductAsync(string salesOrderNumber, Product product, int quantity,double price, string doneBy)
        {
            this._productTransactions.Add(new ProductTransaction
            {
                SONumber = salesOrderNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.SellProduct,
                QuantityAfter = product.Quantity - quantity,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
                UnitPrice = price
            });

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactions(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? productTransactionType)
        {
            // get all products so we can use it on th join statement
            var products = (await this.productRepository.GetProductsByNameAsync(string.Empty)).ToList();

            // write the query
            var query = from prodTransactions in _productTransactions
                        join prod in products on prodTransactions.ProductId equals prod.ProductId
                        where (string.IsNullOrWhiteSpace(productName) || prod.ProductName.ToLower().IndexOf(productName.ToLower()) >= 0) &&
                              (!dateFrom.HasValue || prodTransactions.TransactionDate >= dateFrom.Value.Date) &&
                              (!dateTo.HasValue || prodTransactions.TransactionDate <= dateTo.Value.Date) &&
                              (!productTransactionType.HasValue || prodTransactions.ActivityType == productTransactionType)

                        select new ProductTransaction
                        {
                            Product = prod,
                            PONumber = prodTransactions.PONumber,
                            SONumber = prodTransactions.SONumber,
                            QuantityAfter = prodTransactions.QuantityAfter,
                            QuantityBefore = prodTransactions.QuantityBefore,
                            ProductId = prodTransactions.ProductId,
                            UnitPrice = prodTransactions.UnitPrice,
                            TransactionDate = prodTransactions.TransactionDate,
                            DoneBy = prodTransactions.DoneBy,
                        };

            return query;
        }
    }
}
