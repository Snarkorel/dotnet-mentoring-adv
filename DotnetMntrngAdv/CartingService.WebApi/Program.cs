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
            //app.MapGet("/cart/{key}", GetCartItems);

            app.MapPost("/cart/{key}", AddItem);
            app.MapDelete("/cart/{key}/{id}", DeleteItem);

            app.Run();
        }

        //v1 method of GET /cart/{key}
        private static async Task<IResult> GetCartInfo(ICartingService service, string key)
        {
            //TODO
            throw new NotImplementedException();
        }

        //v2 method of GET /cart/{key}
        private static async Task<IResult> GetCartItems(ICartingService service, string key)
        {
            //TODO
            throw new NotImplementedException();
        }

        private static async Task<IResult> AddItem(ICartingService service, string key, [FromBody] CartItem item)
        {
            //TODO
            throw new NotImplementedException();
        }

        private static async Task<IResult> DeleteItem(ICartingService service, string key, int id)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}