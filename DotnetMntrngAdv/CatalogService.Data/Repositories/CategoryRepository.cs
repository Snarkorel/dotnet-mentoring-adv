using CatalogService.Core.Interfaces;
using CatalogService.Data.Mappers;
using CatalogService.Data.Models;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DbContext context) : base(context) { }

        private async Task<Category> GetCategoryEntityAsync(int id)
        {
            return await _context.Set<Category>()
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(x => x.Id == id)
                   ?? throw new InvalidOperationException($"Object with id={id} not found in DB");
        }

        public async Task<CategoryItem> GetCategoryAsync(int id)
        {
            var category = await GetCategoryEntityAsync(id);
            return category.ToCategoryItem();
        }

        public async Task<IEnumerable<CategoryItem>> ListCategoriesAsync()
        {
            var categories = await _context.Set<Category>()
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync();
            return categories.Select(x => x.ToCategoryItem());
        }
        
        public async Task AddCategoryAsync(CategoryItem item)
        {
            await AddAsync(item.ToCategory());
        }

        public async Task UpdateCategoryAsync(CategoryItem item)
        {
            await UpdateAsync(item.ToCategory());
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await GetCategoryEntityAsync(id);

            var childCategories = await _context.Set<Category>().Where(c => c.ParentCategoryId == id).ToListAsync();
            foreach (var childCategory in childCategories)
            {
                childCategory.ParentCategory = null;
            }

            _context.RemoveRange(category.Products);
            _context.Set<Category>().Remove(category);

            await _context.SaveChangesAsync();
        }
    }
}
