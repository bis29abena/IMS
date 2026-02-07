using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class ProductRepository : IProductRepository
    {
        private readonly IInventoryRepository inventoryRepository;
        private List<Product> _products;

        public ProductRepository(IInventoryRepository inventoryRepository)
        {
            _products = new List<Product>() 
            { 
                new Product{ProductId = 1, ProductName = "Bike", Quantity = 10, Price = 150},
                new Product{ProductId = 2, ProductName = "Car", Quantity = 10, Price = 2500}
            };
            this.inventoryRepository = inventoryRepository;
        }

        public Task AddProductAsync(Product product)
        {
            if(_products.Any(inv => inv.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase))) { return  Task.CompletedTask; }

            var maxId = _products.Max(inv => inv.ProductId);

            product.ProductId = maxId + 1;

            _products.Add(product);

            return Task.CompletedTask;
        }

        public async Task DeleteProductAsync(int productId)
        {
            var productFound = await FindProductAsync(productId);

            if (productFound is not null) 
            {
                _products.Remove(productFound);
            }

            await Task.CompletedTask;
        }

        public async Task<Product?> FindProductAsync(int productId)
        {
            var prod = _products.FirstOrDefault(x => x.ProductId == productId);

            var newProd = new Product();

            if(prod is not null)
            {
                newProd.ProductId = productId;
                newProd.ProductName = prod.ProductName;
                newProd.Quantity = prod.Quantity;
                newProd.Price = prod.Price;
                newProd.ProductInventories = new List<ProductInventory>();

                if(prod.ProductInventories != null && prod.ProductInventories.Count > 0)
                {
                    foreach (var prodInv in prod.ProductInventories)
                    {
                        var newProdInv = new ProductInventory
                        {
                            InventoryID = prodInv.InventoryID,
                            ProductID = prodInv.ProductID,
                            Product = prod,
                            Inventory = new Inventory(),
                            InventoryQuantity = prodInv.InventoryQuantity,
                        };
                        if(prodInv.Inventory != null)
                        {
                            var inv = await this.inventoryRepository.FindInventoryAsync(prodInv.InventoryID);

                            if(inv != null)
                            {
                                newProdInv.Inventory.InventoryId = inv.InventoryId;
                                newProdInv.Inventory.InventoryName = inv.InventoryName;
                                newProdInv.Inventory.Quantity = inv.Quantity;
                                newProdInv.Inventory.Price = inv.Price;
                            }
                        }

                        newProd.ProductInventories.Add(newProdInv);
                    }
                }
            }

            return await Task.FromResult(newProd);
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return await Task.FromResult(_products);

            return _products.Where(x => x.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public Task UpdateProduct(Product product)
        {
            if(_products.Any(x => x.ProductId != product.ProductId && 
            x.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase))) 
                return Task.CompletedTask;

            var productFound = _products.Find(x => x.ProductId == product.ProductId);

            if (productFound is not null)
            {
                productFound.ProductName = product.ProductName;
                productFound.Quantity = product.Quantity;
                productFound.Price = product.Price;
                productFound.ProductInventories = product.ProductInventories;
            }

            return Task.CompletedTask;
        }
    }
}
