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

        public async Task<CategoryItem> GetCategoryAsync(int id)
        {
            var category = await GetAsync(id);
            return category.ToCategoryItem();
        }

        public async Task<IEnumerable<CategoryItem>> ListCategoriesAsync()
        {
            var categories = await ListAsync();
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
            await DeleteAsync(id);
        }
    }
}
