using System.ComponentModel.DataAnnotations;

namespace Application.Models.Products
{
    public class UpdateProductPriceDto
    {
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? ProductPrice { get; set; }
    }
}
