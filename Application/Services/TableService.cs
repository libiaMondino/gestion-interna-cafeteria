using Application.Interfaces;
using Application.Models.Tables;
using Domain.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class TableService : ITableService
    {
        private readonly IGenericRepository<Table> _repository;
        public TableService(IGenericRepository<Table> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TableDto>> GetAllAsync() 
        {
            var tables = await _repository.GetAllAsync();
            return tables.Select( t => new TableDto
            {
                Id = t.Id,
                Number = t.Number,
                Capacity = t.Capacity,
                Status = t.Status
            }
            );
        }
        public async Task<TableDto?> GetByNumberAsync(int num)
        {
            if (num <= 0) throw new ArgumentException("El número de mesa debe ser mayor a 0.");
            
            var table = await _repository.GetByFilterAsync(t => t.Number == num);

            if (table == null) 
                throw new KeyNotFoundException($"No se ha encontrado mesa con el número {num}");
            return new TableDto
            {
                Id = table.Id,
                Number = table.Number,
                Capacity = table.Capacity,
                Status = table.Status
            }; 
        }

        public async Task<IEnumerable<TableDto>> GetOccupiedTablesAsync()
        {
            var status = "Ocupada";
            var tables = await _repository.GetListByFilterAsync(t => t.Status == status);
            return tables.Select(t => new TableDto
            {
                Id = t.Id,
                Number = t.Number,
                Capacity = t.Capacity,
                Status = t.Status
            });
        }

        public async Task<IEnumerable<TableDto>> GetReleasedTablesAsync()
        {
            var status = "Libre";
            var tables = await _repository.GetListByFilterAsync(t => t.Status == status);
            return tables.Select(t => new TableDto
            {
                Id = t.Id,
                Number = t.Number,
                Capacity = t.Capacity,
                Status = t.Status
            });
        }

        public async Task UpdateAsync(int id, UpdateTableDto objModel)
        {
            var table = await _repository.GetIdAsync(id);
            if(table is null)
                throw new KeyNotFoundException($"No se ha encontrado la mesa con el id {id}");
            
            // Validación si hay número duplicado
            // Se excluye la mesa que el usuario está editando ( por medio del id)
            var numDuplicated = await _repository.GetByFilterAsync(t => objModel.Number == t.Number && t.Id != id);
            if (numDuplicated is not null)
                throw new ArgumentException("Número de mesa duplicado"); 

            table.Number = objModel.Number;
            table.Capacity = objModel.Capacity;

            // Solo cambiamos el estado si realmente es diferente al que ya tiene la mesa
            // Se modifica de esta forma porque se configuró como private set
            if (table.Status != objModel.Status)
            {
                
                if (objModel.Status == "Libre")
                {
                    table.Release();
                }
                else if (objModel.Status == "Ocupada")
                {
                    table.Occupy();
                }
                else
                {
                    throw new ArgumentException("El estado debe ser 'Libre' u 'Ocupada'");
                }
            }
            _repository.Update(table);
            
        }

       public async Task RemoveAsync(int id)
       {
            var table = await _repository.GetIdAsync(id);
            if (table is null)
                throw new KeyNotFoundException($"No se ha encontrado la mesa con el id {id}");
            _repository.Remove(table);
       }

       public async Task OccupyAsync(int id)
       {
            var table = await _repository.GetIdAsync(id);
            if (table is null)
                throw new KeyNotFoundException($"No se ha encontrado la mesa con el id {id}");
            table.Occupy();
            _repository.Update(table);
       }

        public async Task ReleaseAsync(int id)
        {
            var table = await _repository.GetIdAsync(id);
            if (table is null)
                throw new KeyNotFoundException($"No se ha encontrado la mesa con el id {id}");
            table.Release();
            _repository.Update(table);
        }

        public async Task AddAsync(CreateTableDto objModel)
        {
            // Validación si hay número duplicado
            var numDuplicated = await _repository.GetByFilterAsync(t => objModel.Number == t.Number);
            if (numDuplicated is not null)
                throw new ArgumentException("Número de mesa duplicado");

            var table = new Table(objModel.Number, objModel.Capacity); // El Status es por default "Libre"
           
            _repository.Add(table);

        }


    }

}
