namespace Application.Models.Categories
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

//datos que recibirá nuestra API cuando alguien quiera crear una categoría nueva