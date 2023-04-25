using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Infrastructure.ServiceBus.DTO;
using Infrastructure.ServiceBus.Interfaces;

namespace Infrastructure.ServiceBus
{
    public class MessageListener : MessagingClient, IMessageListener
    {
        private readonly ServiceBusProcessor _processor;
        private Action<ItemDto> _callback;

        public MessageListener()
        {
            _processor = Client.CreateProcessor(QueueName);
        }
        
        public async Task Subscribe(Action<ItemDto> handler)
        {
            _callback = handler;
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();
        }

        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var payload = args.Message.Body.ToString();

            //TODO: error processing
            var dto = JsonSerializer.Deserialize<ItemDto>(payload);
            _callback.Invoke(dto);
            
            await args.CompleteMessageAsync(args.Message);
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            //TODO: error logging and processing
            Console.WriteLine(args.ErrorSource);
            Console.WriteLine(args.EntityPath);
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
