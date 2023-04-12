using CatalogService.Domain.Entities;

namespace CatalogService.Core.Interfaces
{
    public interface ICategoryRepository
    {
        Task<CategoryItem> GetCategoryAsync(int id);

        Task<IEnumerable<CategoryItem>> ListCategoriesAsync();

        Task<int> AddCategoryAsync(CategoryItem item);

        Task UpdateCategoryAsync(CategoryItem item);

        Task DeleteCategoryAsync(int id);
    }
}
