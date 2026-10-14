using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();// lista todos 

        Task<T?> GetByIdAsync(int id);// busca por id           

        Task AddAsync(T entity); // crea

        void Update(T entity); //modifica

        void Delete(T entity);// elimina
    }
}
