using Application.Interfaces;
using Application.Models.Tables;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class TableService : ITableService
    {
        private readonly IGenericRepository _repository;
        public TableService(IGenericRepository repository)
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

            if (table == null) return null;
            return new TableDto
            {
                Id = table.Id,
                Number = table.Number,
                Capacity = table.Capacity,
                Status = table.Status
            }; 
        }



    }
}
