using System.ComponentModel.DataAnnotations;

namespace Application.Models.Categories
{
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
