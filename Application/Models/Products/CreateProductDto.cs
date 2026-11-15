using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Models.Products
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? ProductDescription { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal ProductPrice { get; set; }

        [EnumDataType(typeof(ProductType))]
        public ProductType ProductType { get; set; }

        public bool ProductIsAvailable { get; set; }

        [Range(0, int.MaxValue)]
        public int? ProductQuantityStock { get; set; }

        [Range(0, int.MaxValue)]
        public int? ProductMinimumStock { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
