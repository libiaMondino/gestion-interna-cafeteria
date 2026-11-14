using System;
using System.Data;
using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Transactions
{
    public class TransactionManager : ITransactionManager
    {
        private readonly ApplicationDBContext _context;

        public TransactionManager(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            ArgumentNullException.ThrowIfNull(operation);

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                var result = await operation();

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return result;
            }
            catch
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                finally
                {
                    _context.ChangeTracker.Clear();
                }

                throw;
            }
        }
    }
}