
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        protected readonly ApplicationDBContext _context;

        public GenericRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // Obtener una entidad por su ID
        public async Task<TEntity?> GetIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        // Obtener todas las entidades
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        // Obtener una entidad según una condición
        public async Task<TEntity?> GetByFilterAsync(
            Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>()
                .FirstOrDefaultAsync(predicate);
        }

        // Obtener una lista según una condición
        public async Task<IEnumerable<TEntity>> GetListByFilterAsync(
            Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>()
                .Where(predicate)
                .ToListAsync();
        }

        // Agregar una entidad
        public void Add(TEntity objModel)
        {
            _context.Set<TEntity>().Add(objModel);
            _context.SaveChanges();
        }

        // Agregar varias entidades
        public void AddRange(IEnumerable<TEntity> objModel)
        {
            _context.Set<TEntity>().AddRange(objModel);
            _context.SaveChanges();
        }

        // Actualizar una entidad
        public void Update(TEntity objModel)
        {
            _context.Entry(objModel).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // Eliminar una entidad
        public void Remove(TEntity objModel)
        {
            _context.Set<TEntity>().Remove(objModel);
            _context.SaveChanges();
        }

        // Contar entidades, con o sin filtro
        public async Task<int> FilteredCountAsync(
            Expression<Func<TEntity, bool>>? predicate = null)
        {
            if (predicate == null)
            {
                return await _context.Set<TEntity>().CountAsync();
            }

            return await _context.Set<TEntity>()
                .CountAsync(predicate);
        }

        // Obtener todas las entidades incluyendo relaciones
        public async Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(
            params string[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        // Obtener una entidad con sus relaciones según un filtro
        public async Task<TEntity?> GetByFilterWithIncludesAsync(
            Expression<Func<TEntity, bool>> predicate,
            params string[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }
    }
}
