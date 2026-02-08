using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels.ViewModelsValidation
{
    public class Sell_EnsureEnoughProductQuantity : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var SellProd = validationContext.ObjectInstance as SellProductViewModel;

            if (SellProd != null)
            {
                if(SellProd.Product is not null && SellProd.QuantityToSell > SellProd.Product.Quantity)
                {
                    return new ValidationResult($"The product {SellProd.Product.ProductName} is not enough and it has only {SellProd.Product.Quantity} in the warehouse!!!", new[] { validationContext.MemberName});
                }
            }

            return ValidationResult.Success;
        }
    }
}
