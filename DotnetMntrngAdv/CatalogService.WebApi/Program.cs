using CatalogService.Core.Interfaces;
using CatalogService.Core.Queries.Filters;
using CatalogService.Data.Database;
using CatalogService.Data.Repositories;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                .AddDbContext<DbContext, CatalogContext>(ServiceLifetime.Transient)
                .AddSingleton<ICategoryRepository, CategoryRepository>()
                .AddSingleton<IProductRepository, ProductRepository>()
                .AddSingleton<ICatalogService, Core.CatalogService>();
            var app = builder.Build();

            app.MapGet("/categories", GetAllCategories);
            app.MapGet("/category/{id}", GetCategory);
            app.MapPost("/category/", AddCategory);
            app.MapPut("/category/{id}", UpdateCategory);
            app.MapDelete("/category/{id}", DeleteCategory);

            app.MapGet("/items", GetAllItems);
            app.MapGet("/item/{id}", GetItem);
            app.MapPost("/item/", AddItem);
            app.MapPut("/item/{id}", UpdateItem);
            app.MapDelete("/item/{id}", DeleteItem);

            app.Run();
        }

        private static async Task<IResult> GetAllCategories(ICatalogService catalogService)
        {
            return TypedResults.Ok(await catalogService.ListCategories());
        }

        private static async Task<IResult> GetCategory(ICatalogService catalogService, int id)
        {
            try
            {
                var category = await catalogService.GetCategory(id);
                return TypedResults.Ok(category);
            }
            catch (InvalidOperationException e)
            {
                return TypedResults.NotFound();
            }
        }

        private static async Task<IResult> AddCategory(ICatalogService catalogService, CategoryItem category)
        {
            throw new NotImplementedException();
            //return TypedResults.Created();
        }

        private static async Task<IResult> UpdateCategory(ICatalogService catalogService, CategoryItem category)
        {
            throw new NotImplementedException();
            //return TypedResults.NotFound();
            //return TypedResults.NoContent();
        }

        private static async Task<IResult> DeleteCategory( ICatalogService catalogService, int id)
        {
            throw new NotImplementedException();
            //return TypedResults.Ok();
            //return TypedResults.NotFound();
        }

        private static async Task<IResult> GetAllItems(ICatalogService catalogService, ProductFilter filter)
        {
            return TypedResults.Ok(await catalogService.ListProductsPaged(filter));
        }

        private static async Task<IResult> GetItem(ICatalogService catalogService, int id)
        {
            try
            {
                var category = await catalogService.GetProduct(id);
                return TypedResults.Ok(category);
            }
            catch (InvalidOperationException e)
            {
                return TypedResults.NotFound();
            }
        }

        private static async Task<IResult> AddItem(ICatalogService catalogService, ProductItem item)
        {
            throw new NotImplementedException();
            //return TypedResults.Created();
        }

        private static async Task<IResult> UpdateItem(ICatalogService catalogService, ProductItem item)
        {
            throw new NotImplementedException();
            //return TypedResults.NotFound();
            //return TypedResults.NoContent();
        }

        private static async Task<IResult> DeleteItem(ICatalogService catalogService, int id)
        {
            throw new NotImplementedException();
            //return TypedResults.Ok();
            //return TypedResults.NotFound();
        }
    }
}