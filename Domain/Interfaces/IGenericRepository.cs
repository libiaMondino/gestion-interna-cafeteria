using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
