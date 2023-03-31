using CatalogService.Data.Models;
using CatalogService.Domain.Entities;

namespace CatalogService.Data.Mappers
{
    public static class CategoryMapper
    {
        public static Category ToCategory(this CategoryItem item)
        {
            return new Category
            {
                Id = item.Id,
                Name = item.Name,
                Image = item.Image,
                ParentCategoryId = item.ParentCategory?.Id
            };
        }

        public static CategoryItem ToCategoryItem(this Category category)
        {
            return new CategoryItem
            {
                Id = category.Id,
                Name = category.Name,
                Image = category.Image,
                ParentCategory = category.ParentCategory?.ToCategoryItem()
            };
        }
    }
}
