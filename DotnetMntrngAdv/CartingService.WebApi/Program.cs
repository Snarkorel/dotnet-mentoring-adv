using CartingService.Core.Entities;
using CartingService.Core.Interfaces;
using CartingService.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CartingService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                .AddLogging()
                .AddSingleton<ICartRepository, CartRepository>()
                .AddSingleton<ICartingService, Core.CartingService>();
            var app = builder.Build();

            //TODO: add Swagger
            //TODO: add self-documentaion

            app.MapGet("/cart/{key}", GetCartInfo);
            //TODO: add versioning support
            app.MapGet("/cart/{key}/TEMP", GetCartItems); //TODO

            app.MapPost("/cart/{key}", AddItem);
            app.MapDelete("/cart/{key}/{id}", DeleteItem);

            app.Run();
        }

        //v1 method of GET /cart/{key}
        private static async Task<IResult> GetCartInfo(ICartingService service, string key)
        {
            try
            {
                var cart = await service.GetCartInfo(key);
                return TypedResults.Ok(cart);
            }
            catch (ArgumentException)
            {
                return TypedResults.NotFound();
            }
        }

        //v2 method of GET /cart/{key}
        private static async Task<IResult> GetCartItems(ICartingService service, string key)
        {
            try
            {
                var cart = await service.GetCartInfo(key);
                return TypedResults.Ok(cart.Items);
            }
            catch (ArgumentException)
            {
                return TypedResults.NotFound();
            }
        }

        private static async Task<IResult> AddItem(ICartingService service, string key, [FromBody] CartItem item)
        {
            try
            {
                await service.AddItem(key, item);
                return TypedResults.Ok();
            }
            catch (Exception ex) //Note: exception details exposed for debugging reasons
            {
                return TypedResults.Problem(new ProblemDetails { Detail = ex.StackTrace, Title = ex.Message, Status = 500 });
            }
        }

        private static async Task<IResult> DeleteItem(ICartingService service, string key, int id)
        {
            try
            {
                await service.RemoveItem(key, id);
                return TypedResults.Ok();
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(new ProblemDetails { Detail = ex.StackTrace, Title = ex.Message, Status = 500 });
            }
        }
    }
}