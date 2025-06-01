using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace doft.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.UserCategories)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .Include(c => c.UserCategories)
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<Category?> GetByNameAndUserIdAsync(string name, string userId)
        {
            return await _context.Categories
                .Include(c => c.UserCategories)
                .FirstOrDefaultAsync(c => c.Name == name && c.UserCategories.Any(uc => uc.UserId == userId));
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.UserCategories)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Categories
                .Include(c => c.UserCategories)
                .Where(c => c.UserCategories.Any(uc => uc.UserId == userId))
                .ToListAsync();
        }

        public async Task<Category> AddAsync(Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            category.UpdatedAt = DateTime.UtcNow;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string name, string userId)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name == name && c.UserCategories.Any(uc => uc.UserId == userId));
        }

        public async Task<UserCategory> AddUserCategoryAsync(UserCategory userCategory)
        {
            userCategory.CreatedAt = DateTime.UtcNow;
            await _context.UserCategories.AddAsync(userCategory);
            await _context.SaveChangesAsync();
            return userCategory;
        }

        public async Task<UserCategory?> GetUserCategoryAsync(int categoryId, string userId)
        {
            return await _context.UserCategories
                .FirstOrDefaultAsync(uc => uc.CategoryId == categoryId && uc.UserId == userId);
        }

        public async Task DeleteUserCategoryAsync(UserCategory userCategory)
        {
            _context.UserCategories.Remove(userCategory);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetCategoriesForUserAsync(string userId)
        {
            return await _context.UserCategories
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.Category)
                .ToListAsync();
        }
    }
}