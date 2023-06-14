using CartingService.Core.Interfaces;
using CartingService.Core.Entities;
using CartingService.Persistence.Repositories;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Queries.Filters;
using CatalogService.Data.Database;
using CatalogService.Data.Repositories;
using CatalogService.Domain.Entities;
using Infrastructure.ServiceBus;
using Infrastructure.ServiceBus.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;

namespace TestApp
{
    internal class Program
    {
        private static ICartingService? _cartingService;
        private static ICatalogService? _catalogService;

        static void Main(string[] args)
        {
            Console.WriteLine("TestApp initialized");
            //TestCartingService().Wait();
            //TestCatalogService().Wait();
            //TestMessaging().Wait();
            //TestCatalogServiceAuthorization().Wait();
            //TestCartingServiceAuthorization().Wait();
            TestGrpcService().Wait();
        }

        private static void PrintCartItem(CartItem item)
        {
            Console.WriteLine($"[CartItem]\r\nId = {item.Id}\r\nName = {item.Name}\r\nImage URL = {item.Image}\r\nPrice = {item.Price}\r\nQuantity = {item.Quantity}\r\n");
        }

        private static void PrintItems(IEnumerable<CartItem> items)
        {
            Console.WriteLine("\r\nItems in cart:");
            foreach (var item in items)
            {
                PrintCartItem(item);
            }
        }

        private static async Task DeleteItems(ICartingService service, string cartKey)
        {
            Console.WriteLine("Removing items...");
            var cart = await service.GetCartInfo(cartKey);
            foreach (var item in cart.Items)
            {
                Console.WriteLine($"Removing item with id={item.Id}");
                await service.RemoveItem(cartKey, item.Id);
            }
        }

        private static async Task GetAndPrintItems(ICartingService service, string cartKey)
        {
            var cart = await service.GetCartInfo(cartKey);
            Console.WriteLine($"Cart contents [key={cart.Key}]:");
            PrintItems(cart.Items);
        }

        private static ICartingService GetCartingService()
        {
            if (_cartingService == null)
            {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ICartRepository, CartRepository>()
                .AddSingleton<IMessageListener, MessageListener>()
                .AddSingleton<ICartingService, CartingService.Core.CartingService>()
                .BuildServiceProvider();

                _cartingService = serviceProvider.GetService<ICartingService>();
            }

            return _cartingService;
        }

        private static async Task TestCartingService()
        {
            Console.WriteLine("Testing Carting Service");

            var cartingService = GetCartingService();

            var cartKey = "cart123";
            var firstItem = new CartItem
            {
                Id = 1,
                Image = new Uri("https://google.com/logo.png"),
                Name = "testname",
                Price = 10.25m,
                Quantity = 1
            };
            Console.WriteLine($"Initializing cart [{cartKey} with one item]");
            await cartingService.AddItem(cartKey, firstItem);
            await GetAndPrintItems(cartingService, cartKey);

            Console.WriteLine("Adding another item");
            var secondItem = new CartItem 
            {
                Id = 2,
                Name = "Second test item", 
                Price = 100.500m, 
                Quantity = 15

            };
            await cartingService.AddItem(cartKey, secondItem);
            await GetAndPrintItems(cartingService, cartKey);

            Console.WriteLine("Removing first item");
            var cartInfo = await cartingService.GetCartInfo(cartKey);
            firstItem = cartInfo.Items.First();
            await cartingService.RemoveItem(cartKey, firstItem.Id);
            await GetAndPrintItems(cartingService, cartKey);

            Console.WriteLine("Removing second item");
            cartInfo = await cartingService.GetCartInfo(cartKey);
            secondItem = cartInfo.Items.Last();
            await cartingService.RemoveItem(cartKey, secondItem.Id);
            await GetAndPrintItems(cartingService, cartKey);
        }

        private static async Task CartingServiceCleanup(ICartingService cartingService, string cartKey)
        {
            var cartInfo = await cartingService.GetCartInfo(cartKey);
            var items = cartInfo.Items;
            foreach (var item in items)
            {
                await cartingService.RemoveItem(cartKey, item.Id);
            }
            await GetAndPrintItems(cartingService, cartKey);
        }

        private static void PrintCategory(CategoryItem item)
        {
            Console.WriteLine(
                $"[Category]\r\nId = {item.Id}\r\nName = {item.Name}\r\nImage = {item.Image}\r\nParent Category = {item.ParentCategory?.Id} | {item.ParentCategory?.Name}");
        }

        private static void PrintCategories(IEnumerable<CategoryItem> items)
        {
            Console.WriteLine("\r\nCategories:");
            foreach (var item in items)
            {
                PrintCategory(item);
            }
        }

        private static async Task GetAndPrintCategories(ICatalogService service)
        {
            var categories = await service.ListCategories();
            PrintCategories(categories);
        }

        private static async Task CleanupCategories(ICatalogService service)
        {
            Console.WriteLine("Cleaning up categories...");
            var categories = await service.ListCategories();
            foreach (var category in categories)
            {
                Console.WriteLine($"Cleaning up category with id={category.Id}");
                await service.DeleteCategory(category.Id);
            }
        }

        private static void PrintProduct(ProductItem item)
        {
            Console.WriteLine(
                $"[Product]\r\nId = {item.Id}\r\nName = {item.Name}\r\nDescription = {item.Description}\r\nImage = {item.Image}\r\nCategory = [{item.Category.Id}] {item.Category.Name}\r\nPrice = {item.Price}\r\nAmount = {item.Amount}");
        }

        private static void PrintProducts(IEnumerable<ProductItem> items)
        {
            Console.WriteLine("\r\nItems:");
            foreach (var item in items)
            {
                PrintProduct(item);
            }
        }

        private static async Task GetAndPrintProducts(ICatalogService service)
        {
            var products = await service.ListProducts();
            PrintProducts(products);
        }

        private static async Task CleanupProducts(ICatalogService service)
        {
            Console.WriteLine("Cleaning up products...");
            var products = await service.ListProducts();
            foreach (var product in products)
            {
                Console.WriteLine($"Cleaning up product with id={product.Id}");
                await service.DeleteProduct(product.Id);
            }
        }

        private static async Task TestCategories(ICatalogService catalogService)
        {
            Console.WriteLine("Categories methods");

            Console.WriteLine("Listing categories");
            await GetAndPrintCategories(catalogService);
            
            Console.WriteLine("Adding category");
            var category = new CategoryItem
            {
                Name = "Main category", 
                Image = "http://localhost/img.png", 
                ParentCategory = null
            };
            await catalogService.AddCategory(category);
            await GetAndPrintCategories(catalogService);

            Console.WriteLine("Modifying category");
            var categories = await catalogService.ListCategories();
            var id = categories.First().Id;
            category = await catalogService.GetCategory(id);
            category.Image = "http://localhost/image.png";
            await catalogService.UpdateCategory(category);
            await GetAndPrintCategories(catalogService);

            Console.WriteLine("Adding nested category");
            var parentCategory = new CategoryItem
            {
                Name = "Parent category"
            };
            await catalogService.AddCategory(parentCategory);

            categories = await catalogService.ListCategories();
            parentCategory = categories.Last();
            var nestedCategory = new CategoryItem
            {
                Name = "Child category",
                Image = "http://127.0.0.1/logo.png",
                ParentCategory = parentCategory
            };
            await catalogService.AddCategory(nestedCategory);
            await GetAndPrintCategories(catalogService);
            
            Console.WriteLine("Categories get/modify methods test completed");
        }

        private static async Task TestProducts(ICatalogService catalogService)
        {
            Console.WriteLine("Products methods");

            Console.WriteLine("Listing products");
            await GetAndPrintProducts(catalogService);
            
            Console.WriteLine("Adding product");
            var categories = await catalogService.ListCategories();
            var category = categories.Last();
            var product = new ProductItem
            {
                Name = "Soap", 
                Description = "Regular hygienic soap", 
                Image = "http://localhost/soap.png", 
                Category = category, 
                Amount = 2,
                Price = 12.5m
            };
            await catalogService.AddProduct(product);
            await GetAndPrintProducts(catalogService);
            
            Console.WriteLine("Modifying product");
            var products = await catalogService.ListProducts();
            var id = products.First().Id;
            product = await catalogService.GetProduct(id);
            product.Price = 16.25m;
            await catalogService.UpdateProduct(product);
            await GetAndPrintProducts(catalogService);

            Console.WriteLine("Products get/modify methods test completed");
        }

        private static async Task TestProductsRemovalWithCategory(ICatalogService catalogService)
        {
            Console.WriteLine("Testing Products removal on Category deletion");

            Console.WriteLine("Listing products");
            await GetAndPrintProducts(catalogService);

            Console.WriteLine("Creating category");
            var newCategory = new CategoryItem
            {
                Name = "Some category"
            };
            await catalogService.AddCategory(newCategory);
            var categories = await catalogService.ListCategories();
            var category = categories.First();
            

            Console.WriteLine("Creating and adding products");
            var firstItem = new ProductItem
            {
                Name = "First item",
                Price = 100m,
                Amount = 1,
                Category = category
            };

            var secondItem = new ProductItem
            {
                Name = "Second Item",
                Price = 0.01m,
                Amount = 2,
                Category = category
            };
            await catalogService.AddProduct(firstItem);
            await catalogService.AddProduct(secondItem);

            Console.WriteLine("Performing ListProductsPaged tests");
            Console.WriteLine("Pagination test: Page number = 1, Page size = 1");
            var filter = new ProductFilter {PageNumber = 1, PageSize = 1};
            var pagedProducts = await catalogService.ListProductsPaged(filter);
            PrintProducts(pagedProducts);

            Console.WriteLine("Pagination test: Page number = 2, Page size = 1");
            filter.PageNumber = 2;
            pagedProducts = await catalogService.ListProductsPaged(filter);
            PrintProducts(pagedProducts);

            var products = await catalogService.ListProducts();
            var firstProductId = products.Select(x => x.Id).First();
            Console.WriteLine($"Pagination test: Page number = 1, Page size = 10, Id (filter) = {firstProductId}");
            filter.CategoryId = category.Id;
            filter.PageNumber = 1;
            filter.PageSize = 10;
            pagedProducts = await catalogService.ListProductsPaged(filter);
            PrintProducts(pagedProducts);

            await GetAndPrintCategories(catalogService);
            await GetAndPrintProducts(catalogService);

            Console.WriteLine("Removing all categories");
            await CleanupCategories(catalogService);

            Console.WriteLine("Checking, if we still have products");
            await GetAndPrintProducts(catalogService);
        }

        private static async Task CatalogServiceCleanup(ICatalogService catalogService)
        {
            await CleanupProducts(catalogService);
            await CleanupCategories(catalogService);
        }

        private static ICatalogService GetCatalogService()
        {
            if (_catalogService == null)
            {
            var serviceProvider = new ServiceCollection()
                    .AddLogging()
                .AddDbContext<DbContext, CatalogContext>(ServiceLifetime.Transient)
                .AddSingleton<ICategoryRepository, CategoryRepository>()
                .AddSingleton<IProductRepository, ProductRepository>()
                .AddSingleton<IMessagePublisher, MessagePublisher>()
                .AddSingleton<ICatalogService, CatalogService.Core.CatalogService>()
                .BuildServiceProvider();

                _catalogService = serviceProvider.GetService<ICatalogService>();
            }

            return _catalogService;
        }

        private static async Task TestCatalogService()
        {
            Console.WriteLine("Testing Catalog Service");

            var catalogService = GetCatalogService();

            await TestCategories(catalogService);
            await TestProducts(catalogService);

            await CatalogServiceCleanup(catalogService);

            await TestProductsRemovalWithCategory(catalogService);

            await CatalogServiceCleanup(catalogService);
        }

        private static async Task TestMessaging()
        {
            Console.WriteLine("Testing messaging");

            var catalogService = GetCatalogService();
            var cartingService = GetCartingService();

            var cartName = "TestCart";
            var category = new CategoryItem
            {
                Name = "Base category"
            };
            await catalogService.AddCategory(category);
            var categories = await catalogService.ListCategories();
            var actualCategory = categories.First();

            var product = new ProductItem
            {
                Name = "ServiceBusTest",
                Image = "http://localhost/img.jpg",
                Price = 10.20m,
                Amount = 2,
                Category = actualCategory
            };
            await catalogService.AddProduct(product);
            var products = await catalogService.ListProducts();
            var actualProduct = products.First();

            var cartItem = new CartItem
            {
                Id = actualProduct.Id,
                Image = new Uri(actualProduct.Image),
                Name = actualProduct.Name,
                Price = actualProduct.Price,
                Quantity = (int)actualProduct.Amount
            };
            await cartingService.AddItem(cartName, cartItem);

            actualProduct.Name = "Modified product test for ServiceBus";
            await catalogService.UpdateProduct(actualProduct);

            await Task.Delay(60000);

            await GetAndPrintItems(cartingService, cartName);

            await CatalogServiceCleanup(catalogService);
            await CartingServiceCleanup(cartingService, cartName);
        }

        private static async Task TestCatalogBuyerAuthorization()
        {
            Console.WriteLine("Testing Buyer access");
            
            //Azure AD tenant ID
            var tenantId = "<CHANGE_ME_TENANT>";
            //WebAPI app registration client ID
            var apiAppClientId = "CHANGE_ME_CATALOG_API_CLIENT_ID";
            var scopes = new[] { $"api://{apiAppClientId}/.default" };
            var authority = $"https://login.microsoftonline.com/{tenantId}";

            //Client app registration client ID
            var clientId = "CHANGE_ME_BUYER_CLIENT_ID";
            //Client secret, obviously, shouldn't be hardcoded like this and should be retrieved securely, but for learning task it's OK
            var clientSecret = "CHANGE_ME_BUYER_CLIENT_SECRET";
            
            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority)
                .WithClientSecret(clientSecret)
                .Build();
            var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();

            if (authResult != null)
            {
                Console.WriteLine("Access token: " + authResult.AccessToken);

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

                var successUrl = "https://localhost:7021/categories";
                var failUrl = "https://localhost:7021/category";

                Console.WriteLine($"Calling {successUrl}");
                string categoriesJson = await client.GetStringAsync(successUrl);
                Console.WriteLine($"Server response:\r\n{categoriesJson}");

                //This should fail - we don't have manager permissions
                var category = "{\r\n\t\"name\": \"post category\",\r\n\t\"image\": \"http://localhost/img.png\",\r\n\t\"parentCategory\": null\r\n}";
                var content = new StringContent(category, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(failUrl, content);
                var responseCode = response.StatusCode;
                if (responseCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine(
                        "Got 403 Forbidden response code while trying to do Manager-only action having Buyer permissions");
                }
                else
                {
                    Console.WriteLine($"Unexpected response code: {responseCode}");
                }
            }
            else
            {
                Console.WriteLine("Failed to acquire token!");
            }
        }

        private static async Task TestCatalogManagerAuthorization()
        {
            Console.WriteLine("Testing Manager access");

            //Azure AD tenant ID
            var tenantId = "<CHANGE_ME_TENANT>";
            //WebAPI app registration client ID
            var apiAppClientId = "CHANGE_ME_CATALOG_API_CLIENT_ID";
            var scopes = new[] { $"api://{apiAppClientId}/.default" };
            var authority = $"https://login.microsoftonline.com/{tenantId}";

            //Client app registration client ID
            var clientId = "CHANGE_ME_MANAGER_CLIENT_ID";
            var clientSecret = "CHANGE_ME_MANAGER_CLIENT_SECRET";

            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority)
                .WithClientSecret(clientSecret)
                .Build();
            var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();

            if (authResult != null)
            {
                Console.WriteLine("Access token: " + authResult.AccessToken);

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

                var addCategoryUrl = "https://localhost:7021/category";
                var getCategoriesUrl = "https://localhost:7021/categories";

                var category = "{\r\n\t\"name\": \"post category\",\r\n\t\"image\": \"http://localhost/img.png\",\r\n\t\"parentCategory\": null\r\n}";
                var content = new StringContent(category, Encoding.UTF8, "application/json");
                Console.WriteLine($"Calling {addCategoryUrl}");
                var response = await client.PostAsync(addCategoryUrl, content);

                Console.WriteLine(response.IsSuccessStatusCode
                    ? "Successfully added new category using Manager client app"
                    : "Failed to add new category from Manager client app");

                Console.WriteLine($"Calling {getCategoriesUrl}");
                var categoriesJson = await client.GetStringAsync(getCategoriesUrl);
                Console.WriteLine($"Server response:\r\n{categoriesJson}");
            }
            else
            {
                Console.WriteLine("Failed to acquire token!");
            }
        }

        private static async Task TestCatalogServiceAuthorization()
        {
            Console.WriteLine("Testing Catalog Service authorization");

            await TestCatalogBuyerAuthorization();
            await TestCatalogManagerAuthorization();
        }

        private static async Task TestCartingCommonFlow(AuthenticationResult authResult)
        {
            Console.WriteLine("Access token: " + authResult.AccessToken);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            var addItemUrl = "https://localhost:7285/cart/testcart123";
            var getCartUrl = "https://localhost:7285/v2/cart/testcart123";

            var item = "{\r\n\t\"name\": \"testname\",\r\n\t\"image\": \"https://google.com/logo.png\",\r\n\t\"price\": 10.25,\r\n\t\"quantity\": 1\r\n}";

            var requestContent = new StringContent(item, Encoding.UTF8, "application/json");
            Console.WriteLine($"Calling {addItemUrl}");
            var addItemResponse = await client.PostAsync(addItemUrl, requestContent);

            if (addItemResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Successfully added new item to cart");
            }
            else
            {
                Console.WriteLine("Failed to add new item to cart, exiting...");
                return;
            }

            Console.WriteLine($"Calling {getCartUrl}");
            var getCartResponse = await client.GetAsync(getCartUrl);
            var responseCode = getCartResponse.StatusCode;

            if (responseCode is HttpStatusCode.OK or HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Successfully completed request to server, response code: {responseCode}");
                var content = await getCartResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Server response:\r\n{content}");
            }
            else
            {
                Console.WriteLine($"Request failed, response code: {responseCode}");
            }
        }

        private static async Task TestCartingBuyerAuthorization()
        {
            Console.WriteLine("Testing Buyer access");

            //Azure AD tenant ID
            var tenantId = "<CHANGE_ME_TENANT>";
            //WebAPI app registration client ID
            var apiAppClientId = "CHANGE_ME_CARTING_API_CLIENT_ID";
            var scopes = new[] { $"api://{apiAppClientId}/.default" };
            var authority = $"https://login.microsoftonline.com/{tenantId}";

            //Client app registration client ID
            var clientId = "CHANGE_ME_BUYER_CLIENT_ID";
            var clientSecret = "CHANGE_ME_BUYER_CLIENT_SECRET";

            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority)
                .WithClientSecret(clientSecret)
                .Build();
            var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            
            if (authResult != null)
            {
                await TestCartingCommonFlow(authResult);
            }
            else
            {
                Console.WriteLine("Failed to acquire token!");
            }
        }

        private static async Task TestCartingManagerAuthorization()
        {
            Console.WriteLine("Testing Manager access");

            //Azure AD tenant ID
            var tenantId = "<CHANGE_ME_TENANT>";
            //WebAPI app registration client ID
            var apiAppClientId = "CHANGE_ME_CARTING_API_CLIENT_ID";
            var scopes = new[] { $"api://{apiAppClientId}/.default" };
            var authority = $"https://login.microsoftonline.com/{tenantId}";

            //Client app registration client ID
            var clientId = "CHANGE_ME_MANAGER_CLIENT_ID";
            var clientSecret = "CHANGE_ME_MANAGER_CLIENT_SECRET";

            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority)
                .WithClientSecret(clientSecret)
                .Build();
            var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();

            if (authResult != null)
            {
                await TestCartingCommonFlow(authResult);
            }
            else
            {
                Console.WriteLine("Failed to acquire token!");
            }
        }

        private static async Task TestCartingServiceAuthorization()
        {
            Console.WriteLine("Testing Carting Service authorization");

            await TestCartingBuyerAuthorization();
            await TestCartingManagerAuthorization();
        }

        private static async Task TestGrpcService()
        {
            const string CartName = "grpc_cart";
            
            using var channel = GrpcChannel.ForAddress("http://localhost:5214");
            var client = new Carting.CartingClient(channel);

            Console.WriteLine("Starting gRPC service test");

            //Add item (client streaming)
            Console.WriteLine("Sending AddItem client streaming request");
            var cancellationToken = new CancellationTokenSource().Token;
            using var addItemCall = client.AddItem(cancellationToken: cancellationToken);
            var itemsToAdd = new RepeatedField<Item>
            {
                new Item
                {
                    Id = 1,
                    Image = "http://localhost/grpc.png",
                    Name = "gRPC test item",
                    Price = "125.75",
                    Quantity = 3
                }
            };

            foreach (var item in itemsToAdd)
            {
                var request = new AddItemRequest { CartName = CartName };
                request.Item.Add(item);
                await addItemCall.RequestStream.WriteAsync(request, cancellationToken);
            }
            await addItemCall.RequestStream.CompleteAsync();
            var addItemsResponse = await addItemCall;
            Console.WriteLine($"AddItem (client streaming) response:\r\n{addItemsResponse}");


            //Get items (unary call)
            Console.WriteLine("Sending GetItems unary request");
            var getItemsRequest = new GetItemsRequest { CartName = CartName };
            var getItemsReply = await client.GetItemsAsync(getItemsRequest);
            Console.WriteLine($"GetItems (server streaming) response:\r\n{getItemsReply}");

            //Add items (bi-directional streaming)
            cancellationToken = new CancellationTokenSource().Token;
            itemsToAdd = new RepeatedField<Item>
            {
                new Item
                {
                    Id = 2,
                    Image = "http://localhost/grpc2.png",
                    Name = "gRPC test item",
                    Price = "200.00",
                    Quantity = 2
                },
                new Item
                {
                    Id = 3,
                    Image = "http://localhost/grpc3.png",
                    Name = "gRPC test item",
                    Price = "300.00",
                    Quantity = 3
                },
                new Item
                {
                    Id = 4,
                    Image = "http://localhost/grpc4.png",
                    Name = "gRPC test item",
                    Price = "400.00",
                    Quantity = 4
                }
            };
            using var addItemBidirectionalCall = client.AddItemStream(cancellationToken: cancellationToken);

            Console.WriteLine("Starting background task to receive AddItem messages");
            var readTask = Task.Run(async () =>
            {
                await foreach (var addItemResponse in addItemBidirectionalCall.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    Console.WriteLine($"Got AddItemReply: {addItemResponse}");
                }
            }, cancellationToken);

            Console.WriteLine("Sending AddItem bi-directional streaming request");
            foreach (var item in itemsToAdd)
            {
                var request = new AddItemRequest { CartName = CartName };
                request.Item.Add(item);
                await addItemBidirectionalCall.RequestStream.WriteAsync(request, cancellationToken);
            }

            Console.WriteLine("Completing AddItem bi-directional streaming request");
            await addItemBidirectionalCall.RequestStream.CompleteAsync();
            await readTask;

            //Get items (server streaming)
            Console.WriteLine("Sending GetItems server streaming request");
            cancellationToken = new CancellationTokenSource().Token;
            var getItemsStreamRequest = new GetItemsRequest { CartName = CartName };
            using var getItemsStreamCall = client.GetItemsStream(getItemsStreamRequest, cancellationToken: cancellationToken);

            while (await getItemsStreamCall.ResponseStream.MoveNext())
            {
                var reply = getItemsStreamCall.ResponseStream.Current;
                Console.WriteLine($"Got streaming reply: {reply}");
            }
            Console.WriteLine("GetItems server streaming request completed");

            Console.WriteLine("gRPC service test completed");
        }
    }
}