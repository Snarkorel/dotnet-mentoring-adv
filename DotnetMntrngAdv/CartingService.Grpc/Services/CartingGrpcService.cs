using Grpc.Core;
using System.Globalization;
using CartingService.Core.Entities;
using CartingService.Core.Interfaces;
using Google.Protobuf.Collections;

namespace CartingService.Grpc.Services
{
    public class CartingGrpcService : Carting.CartingBase
    {
        private readonly ILogger<CartingGrpcService> _logger;
        private readonly ICartingService _cartingService;
        public CartingGrpcService(ILogger<CartingGrpcService> logger, ICartingService cartingService)
        {
            _logger = logger;
            _cartingService = cartingService;
        }

        //TODO: handle service exceptions

        //Unary call
        public override async Task<GetItemsReply> GetItems(GetItemsRequest request, ServerCallContext context)
        {
            _logger.Log(LogLevel.Information, "Acquired GetItems unary request");
            var convertedItems = await GetAndConvertCartItems(request.CartName);

            var response = new GetItemsReply();
            response.Item.AddRange(convertedItems);

            _logger.Log(LogLevel.Information, "Completed GetItems unary request");
            return response;
        }

        //Server streaming
        public override async Task GetItemsStream(GetItemsRequest request, IServerStreamWriter<GetItemsReply> responseStream, ServerCallContext context)
        {
            _logger.Log(LogLevel.Information, "Started GetItems server streaming request");
            var items = await GetCartItems(request.CartName);

            foreach (var item in items)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    return;

                var response = new GetItemsReply();
                response.Item.Add(ConvertToItem(item));

                _logger.Log(LogLevel.Information, $"Sending GetItemsReply: {response}");
                await responseStream.WriteAsync(response);
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }

            _logger.Log(LogLevel.Information, "Completed GetItems server streaming request");
        }
        
        //Client streaming
        public override async Task<AddItemReply> AddItem(IAsyncStreamReader<AddItemRequest> requestStream, ServerCallContext context)
        {
            var cartName = string.Empty;
            _logger.Log(LogLevel.Information, "Started AddItem client streaming request");
            
            while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
            {
                var message = requestStream.Current;
                _logger.Log(LogLevel.Information, $"Processing AddItemRequest message: {message}");

                if (cartName == string.Empty)
                    cartName = message.CartName;

                var requestItems = message.Item;
                foreach (var item in requestItems)
                {
                    var cartItem = ConvertToCartItem(item);
                    await _cartingService.AddItem(cartName, cartItem);
                }
                
            }

            var items = await GetAndConvertCartItems(cartName);
            var response = new AddItemReply();
            response.Item.AddRange(items);

            _logger.Log(LogLevel.Information, "Completed AddItem client streaming request");
            return response;
        }


        //Bidirectional streaming
        public override async Task AddItemStream(IAsyncStreamReader<AddItemRequest> requestStream, IServerStreamWriter<AddItemReply> responseStream, ServerCallContext context)
        {
            var cartName = string.Empty;
            _logger.Log(LogLevel.Information, "Started AddItem bidirectional streaming request");

            var readTask = Task.Run(async () =>
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    foreach (var item in message.Item)
                    {
                        if (cartName == string.Empty)
                            cartName = message.CartName;

                        _logger.Log(LogLevel.Information, $"Processing AddItemRequest message: {message}");
                        await _cartingService.AddItem(message.CartName, ConvertToCartItem(item));
                    }
                }
            });
            
            while (!readTask.IsCompleted)
            {
                var items = await GetAndConvertCartItems(cartName);
                var response = new AddItemReply();
                response.Item.AddRange(items);

                _logger.Log(LogLevel.Information, $"Sending AddItemReply message: {response}");
                await responseStream.WriteAsync(response);
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }

            _logger.Log(LogLevel.Information, "Completed AddItem bidirectional streaming request");
        }

        private static decimal GetDecimal(string value)
        {
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        private static RepeatedField<Item> ConvertItems(IEnumerable<CartItem> items)
        {
            var convertedItems = new RepeatedField<Item>();
            foreach (var item in items)
            {
                convertedItems.Add(ConvertToItem(item));
            }

            return convertedItems;
        }

        private static Item ConvertToItem(CartItem item)
        {
            return new Item
            {
                Id = item.Id,
                Image = item.Image.ToString(),
                Name = item.Name,
                Price = item.Price.ToString(CultureInfo.InvariantCulture),
                Quantity = item.Quantity,
            };
        }

        private static CartItem ConvertToCartItem(Item item)
        {
            return new CartItem
            {
                Id = item.Id,
                Image = new Uri(item.Image),
                Name = item.Name,
                Price = GetDecimal(item.Price),
                Quantity = item.Quantity
            };
        }

        private async Task<IEnumerable<CartItem>> GetCartItems(string cartName)
        {
            var cart = await _cartingService.GetCartInfo(cartName);
            return cart.Items;
        }

        private async Task<RepeatedField<Item>> GetAndConvertCartItems(string cartName)
        {
            var items = await GetCartItems(cartName);
            return ConvertItems(items);
        }
    }
}