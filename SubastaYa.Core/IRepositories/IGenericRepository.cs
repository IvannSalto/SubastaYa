using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Core.IRepositories
{
    public interface IGenericRepository<T> where T : class 
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
