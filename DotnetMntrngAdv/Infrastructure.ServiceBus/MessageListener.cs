using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Infrastructure.ServiceBus.DTO;
using Infrastructure.ServiceBus.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ServiceBus
{
    public class MessageListener : MessagingClient, IMessageListener
    {
        private readonly ServiceBusProcessor _processor;
        private Action<ItemDto> _callback;
        private readonly ILogger<MessageListener> _logger;

        public MessageListener(ILogger<MessageListener> logger)
        {
            _logger = logger;
            _processor = Client.CreateProcessor(QueueName);
        }
        
        public async Task Subscribe(Action<ItemDto> handler)
        {
            _callback = handler;
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
            _logger.LogInformation("Service Bus message listener initialized, staring subscription");
            await _processor.StartProcessingAsync();
        }

        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var payload = args.Message.Body.ToString();
            _logger.LogInformation($"Got a message from Service Bus, payload: {payload}");

            //TODO: error processing
            var dto = JsonSerializer.Deserialize<ItemDto>(payload);
            _callback.Invoke(dto);
            
            await args.CompleteMessageAsync(args.Message);
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            //TODO: error processing
            _logger.LogError(args.Exception, $"An error occurred while processing incoming message [ErrorSource = {args.ErrorSource}, EntityPath = {args.EntityPath}]");
            return Task.CompletedTask;
        }
    }
}
