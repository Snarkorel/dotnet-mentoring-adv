using CatalogService.Core.Interfaces;
using CatalogService.Core.Queries.Filters;
using CatalogService.Data.Database;
using CatalogService.Data.Repositories;
using CatalogService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
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
            //app.MapDelete("/category/{id}", DeleteCategory);

            app.MapGet("/items", GetAllItems);
            app.MapGet("/item/{id}", GetItem);
            app.MapPost("/item/", AddItem);
            app.MapPut("/item/{id}", UpdateItem);
            //app.MapDelete("/item/{id}", DeleteItem);

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
            catch (InvalidOperationException)
            {
                return TypedResults.NotFound();
            }
            catch (Exception ex) //Note: exception details exposed for debugging reasons
            {
                return TypedResults.Problem(new ProblemDetails { Detail = ex.StackTrace, Title = ex.Message, Status = 500 });
            }
        }

        private static async Task<IResult> AddCategory(ICatalogService catalogService, [FromBody] CategoryItem category)
        {
            try
            {
                var id = await catalogService.AddCategory(category);
                return TypedResults.Created($"/category/{id}");
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(new ProblemDetails {Detail = ex.StackTrace, Title = ex.Message, Status = 500});
            }
        }

        private static async Task<IResult> UpdateCategory(ICatalogService catalogService, [FromBody] CategoryItem category)
        {
            try
            {
                await catalogService.UpdateCategory(category);
                return TypedResults.NoContent();
            }
            catch (InvalidOperationException)
            {
                return TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(new ProblemDetails { Detail = ex.StackTrace, Title = ex.Message, Status = 500 });
            }
        }

        //private static async Task<IResult> DeleteCategory( ICatalogService catalogService, int id)
        //{
        //    throw new NotImplementedException();
        //    //return TypedResults.Ok();
        //    //return TypedResults.NotFound();
        //}

        private static async Task<IResult> GetAllItems(ICatalogService catalogService, int pageNumber, int pageSize, int? categoryId)
        {
            return TypedResults.Ok(await catalogService.ListProductsPaged(new ProductFilter {CategoryId = categoryId, PageNumber = pageNumber, PageSize = pageSize}));
        }

        private static async Task<IResult> GetItem(ICatalogService catalogService, int id)
        {
            try
            {
                var item = await catalogService.GetProduct(id);
                return TypedResults.Ok(item);
            }
            catch (InvalidOperationException e)
            {
                return TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(new ProblemDetails { Detail = ex.StackTrace, Title = ex.Message, Status = 500 });
            }
        }

        private static async Task<IResult> AddItem(ICatalogService catalogService, [FromBody] ProductItem item)
        {
            try
            {
                var id = await catalogService.AddProduct(item);
                return TypedResults.Created($"/item/{id}");
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(new ProblemDetails { Detail = ex.StackTrace, Title = ex.Message, Status = 500 });
            }
        }

        private static async Task<IResult> UpdateItem(ICatalogService catalogService, ProductItem item)
        {
            try
            {
                await catalogService.UpdateProduct(item);
                return TypedResults.NoContent();
            }
            catch (ArgumentNullException)
            {
                return TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(new ProblemDetails { Detail = ex.StackTrace, Title = ex.Message, Status = 500 });
            }
        }

        //private static async Task<IResult> DeleteItem(ICatalogService catalogService, int id)
        //{
        //    throw new NotImplementedException();
        //    //return TypedResults.Ok();
        //    //return TypedResults.NotFound();
        //}
    }
}