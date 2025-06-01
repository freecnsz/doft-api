#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Application.Interfaces.RepositoryInterfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id); 
        Task<T> AddAsync(T entity); 
        Task UpdateAsync(T entity); 
        Task DeleteAsync(T entity); 
    }
}