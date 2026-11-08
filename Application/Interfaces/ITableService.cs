using Application.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Application.Interfaces
{
    public interface ITableService
    {
        Task<IEnumerable<TableDto>> GetAllAsync();
        Task<TableDto?> GetByNumberAsync( int num );
        Task<IEnumerable<TableDto>> GetOccupiedTablesAsync();
        Task<IEnumerable<TableDto>> GetReleasedTablesAsync();
        Task UpdateAsync( int id, UpdateTableDto objModel );
        Task RemoveAsync( int id );
        Task OccupyAsync( int id );
        Task ReleaseAsync( int id );
        void Add(CreateTableDto objModel); 
    }
}
