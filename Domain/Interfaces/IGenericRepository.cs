using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetIdAsync(int id); 
        Task<IEnumerable<TEntity>> GetAllAsync();

        // Métodos para obtener con filtros
        Task<TEntity?> GetByFilterAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetListByFilterAsync(Expression<Func<TEntity, bool>> predicate);

        // Otras operaciones CRUD
        // No necesitan ser asincrónicas ya que no interactúan en ese instante con la BDD
        // Entity Framework solo le pone etiquetas como "Added"(Creado)
        void Add(TEntity objModel);
        void AddRange(IEnumerable<TEntity> objModel);
        void Update(TEntity objModel);
        void Remove(TEntity objModel);

        // Para contar todos los registros
        // Ahorra memoria porque obliga a la base de datos a hacer este trabajo y no la app, devolviendo un entero a través de la red
        Task<int> FilteredCountAsync(Expression<Func<TEntity, bool>>? predicate = null);

        // Para liberar de la memoria los recursos que no se necesiten(ej: conexión activa de la BDD)
        void Dispose();
    }
}
