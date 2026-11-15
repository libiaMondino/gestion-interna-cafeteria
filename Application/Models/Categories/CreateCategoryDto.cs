
using System.ComponentModel.DataAnnotations;

namespace Application.Models.Categories
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la categoría no puede superar los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "La descripción de la categoría no puede superar los 255 caracteres.")]
        public string? Description { get; set; }
    }
}

//datos que recibirá nuestra API cuando alguien quiera crear una categoría nueva