using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels.ViewModelsValidation
{
    public class Produce_EnsureEnoughInventoryQuantity : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var produceViewModel = validationContext.ObjectInstance as ProduceProductViewModel;

            if (produceViewModel != null) 
            {
                if(produceViewModel.Product is not null)
                {
                    foreach(var prodInv in produceViewModel.Product.ProductInventories)
                    {
                        if(prodInv is not null && prodInv.InventoryQuantity * produceViewModel.QuantityToProduce > prodInv.Inventory.Quantity)
                        {
                            return new ValidationResult($"The inventory ({prodInv.Inventory.InventoryName}) is not enough to produce {produceViewModel.QuantityToProduce} products",
                                new[] { validationContext.MemberName});
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
