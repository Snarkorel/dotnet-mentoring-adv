using Asp.Versioning;
using CartingService.Core.Entities;
using CartingService.Core.Interfaces;
using CartingService.Persistence.Repositories;
using Infrastructure.ServiceBus;
using Infrastructure.ServiceBus.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CartingService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddAuthentication(
                    JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            builder.Services.AddAuthorization();

            builder.Services
                .AddLogging()
                .AddApplicationInsightsTelemetry()
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "Carting Service API" });
                    c.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "Carting Service API" });
                })
                .AddSingleton<ICartRepository, CartRepository>()
                .AddSingleton<IMessageListener, MessageListener>()
                .AddSingleton<ICartingService, Core.CartingService>()
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(2, 0);
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                });

            builder.Logging.AddDebug();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            //Custom token logging middleware from task 2
            app.Use((context, next) =>
            {
                var token = context.Request.HttpContext.GetTokenAsync("access_token").Result;
                if (!token.IsNullOrEmpty())
                    //just dumb console logging
                    Console.WriteLine($"Request token: {token}");

                return next(context);
            });

            var versionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .HasApiVersion(new ApiVersion(2, 0))
                .ReportApiVersions()
                .Build();

            app.MapGet("/v1/cart/{key}", GetCartInfo)
                .WithOpenApi()
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(1, 0)
                .Produces<Cart>()
                .WithName("GetCartInfo")
                .WithDescription("Returns Cart model with items in it")
                .WithTags("Getters")
                .Produces(StatusCodes.Status404NotFound)
                .WithOpenApi();

            app.MapGet("/v2/cart/{key}", GetCartItems)
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(2, 0)
                .Produces<List<CartItem>>()
                .WithName("GetCartItems")
                .WithDescription("Returns cart items list")
                .WithTags("Getters")
                .Produces(StatusCodes.Status404NotFound)
                .WithOpenApi();

            app.MapPost("/cart/{key}", AddItem)
                .WithApiVersionSet(versionSet)
                .IsApiVersionNeutral()
                .Accepts<CartItem>("application/json")
                .Produces<CartItem>()
                .WithName("AddItem")
                .WithDescription("Adds item to cart. If specified cart doesn't exist - creates it")
                .WithTags("Setters")
                .Produces(StatusCodes.Status500InternalServerError)
                .WithOpenApi();

            app.MapDelete("/cart/{key}/{id}", DeleteItem)
                .WithApiVersionSet(versionSet)
                .IsApiVersionNeutral()
                .WithName("DeleteItem")
                .WithDescription("Deletes item from cart")
                .WithTags("Setters")
                .Produces(StatusCodes.Status500InternalServerError)
                .WithOpenApi();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"v1");
                c.SwaggerEndpoint($"/swagger/v2/swagger.json", $"v2");
            });

            app.Run();
        }

        [Authorize(Roles = "Buyer,Manager")]
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

        [Authorize(Roles = "Buyer,Manager")]
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

        [Authorize(Roles = "Buyer,Manager")]
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

        [Authorize(Roles = "Buyer,Manager")]
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