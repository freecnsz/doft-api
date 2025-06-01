using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Entities;

namespace doft.Application.Interfaces.RepositoryInterfaces
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(int id);
        Task<Category> GetByNameAsync(string name);
        Task<Category> GetByNameAndUserIdAsync(string name, string userId);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<IEnumerable<Category>> GetAllByUserIdAsync(string userId);
        Task<Category> AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<bool> ExistsAsync(string name, string userId);
        Task<UserCategory> AddUserCategoryAsync(UserCategory userCategory);
        Task<UserCategory> GetUserCategoryAsync(int categoryId, string userId);
        Task DeleteUserCategoryAsync(UserCategory userCategory);
        Task<List<Category>> GetCategoriesForUserAsync(string userId);
    }
}