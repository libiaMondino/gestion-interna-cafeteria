using System;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITransactionManager
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> operation);
    }
}