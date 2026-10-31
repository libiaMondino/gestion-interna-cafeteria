using Domain.Interfaces;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
namespace Infrastructure.Repositories
{
    public class GenericRepository<TEntity>: IGenericRepository<TEntity> where TEntity : class
    {
       

        protected readonly ApplicationDBContext _context;

        public GenericRepository(ApplicationDBContext context)
        {
            _context = context; 
        } 
        
        public async Task<TEntity?> GetIdAsync(int id) 
        {
            // Puede devolver nulo
            return await _context.Set<TEntity>().FindAsync(id); 
        }
       
        public async Task<IEnumerable<TEntity>> GetAllAsync() 
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity?> GetByFilterAsync(Expression<Func<TEntity, bool>> predicate) 
        {
            //Devuelve el primer registro que cumpla con la condición(predicate) o null
            return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }
        public async Task<IEnumerable<TEntity>> GetListByFilterAsync(Expression<Func<TEntity, bool>> predicate) 
        {
            // Filtra y devuelve lista
            return await _context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public void Add(TEntity objModel) {
            _context.Set<TEntity>().Add(objModel);
            _context.SaveChanges();
        }
        public void AddRange(IEnumerable<TEntity> objModel) {
            _context.Set<TEntity>().AddRange(objModel);
            _context.SaveChanges();
        }
        public void Update(TEntity objModel) {
           //Entry revisa el objeto y busca su id, para rastrear en la memoria del contexto si existe otra instancia cargada con el mismo id
           //State le indica el estado o la "etiqueta" del obj a Entity Framework, indicando con "Modified" que ese obj existe y que está modificado
            _context.Entry(objModel).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public void Remove(TEntity objModel) {
            //Remove vs Delete 
            //Remove quita elementos de contextos y delete refiere a un comando SQL directo, que impacta en la BDD
            _context.Set<TEntity>().Remove(objModel);
            _context.SaveChanges();
        }

        public async Task<int> FilteredCountAsync(Expression<Func<TEntity, bool>>? predicate = null) 
        {
            // Cuenta cantidad de registros a partir de una condición
            if (predicate == null)
            {
                return await _context.Set<TEntity>().CountAsync();
            }
            return await _context.Set<TEntity>().Where(predicate).CountAsync();
        }

        public void Dispose() {
            // Le está ordenando a la instancia de EF(el DbContext) que ejecute su propia rutina de limpieza(Más allá del Garbage Colector de .NET)
            // 1. Cierra conexiones físicas con la BDD
            // 2. Vacía el Change Tracker(memoria usada para rastrear estados de los obj)
            // 3. Libera cachés internos generados durante consultas de esa sesión
            _context.Dispose();
        }
    }
}
