using Application.Models;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync(); //obtener todas las categorias

        Task<CategoryDto?> GetByIdAsync(int id);// buscar una categoria por su identificador
        //? que puede devolver null
        Task<CategoryDto> CreateAsync(CreateCategoryDto categoryDto); // crear una categoria

        Task<bool> UpdateAsync(int id, UpdateCategoryDto categoryDto);

        Task<bool> DeleteAsync(int id); //solicitar la eliminacion de una categoria
    }
}