using System.ComponentModel.DataAnnotations;

namespace Application.Models.Products
{
    public class ProductStockQuantityDto
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
