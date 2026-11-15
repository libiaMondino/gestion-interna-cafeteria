using System.ComponentModel.DataAnnotations;

namespace Application.Models.Products
{
    public class UpdateProductDto
    {
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? ProductDescription { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
