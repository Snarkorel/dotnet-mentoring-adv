using CatalogService.Data.Models;
using CatalogService.Domain.Entities;

namespace CatalogService.Core.Helpers
{
    public static class CategoryHelper
    {
        //public static IQueryable<CategoryItem> CategoriesToCategoryItems(this IQueryable<Category> categories)
        //{
        //    return categories.Select(x => new CategoryItem
        //    {
        //        Name = x.Name,
        //        Image = new Uri(x.Image),
        //        ParentCategoryId = x.ParentCategoryId
        //    });
        //}

        //public static Category CategoryItemToCategory(CategoryItem item)
        //{
        //    return new Category
        //    {
        //        Name = item.Name,
        //        Image = item.Image.ToString(),
        //        ParentCategoryId = item.ParentCategoryId
        //    };
        //}

        //public static CategoryItem CategoryToCategoryItem(Category category)
        //{
        //    return new CategoryItem
        //    {
        //        Name = category.Name,
        //        Image = new Uri(category.Image),
        //        ParentCategoryId = category.ParentCategoryId
        //    };
        //}
    }
}
