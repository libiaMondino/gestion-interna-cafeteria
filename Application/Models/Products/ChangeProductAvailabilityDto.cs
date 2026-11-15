using System.ComponentModel.DataAnnotations;

namespace Application.Models.Products
{
    public class ChangeProductAvailabilityDto
    {
        [Required]
        public bool? ProductIsAvailable { get; set; }
    }
}
