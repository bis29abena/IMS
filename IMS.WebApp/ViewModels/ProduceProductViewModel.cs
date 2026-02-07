using IMS.CoreBusiness;
using IMS.WebApp.ViewModels.ViewModelsValidation;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels
{
    public class ProduceProductViewModel
    {
        [Required]
        public string ProductionNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "You have selected an Product")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity has to be greater than or equal to 1")]

        [Produce_EnsureEnoughInventoryQuantity]
        public int QuantityToProduce { get; set; }
        public Product? Product { get; set; } = null;
    }
}
