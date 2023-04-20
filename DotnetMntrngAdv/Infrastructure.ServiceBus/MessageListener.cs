using Azure.Messaging.ServiceBus;
using Infrastructure.ServiceBus.DTO;
using Infrastructure.ServiceBus.Interfaces;

namespace Infrastructure.ServiceBus
{
    public class MessageListener : MessagingClient, IMessageListener
    {
        private readonly ServiceBusProcessor _processor;

        public MessageListener()
        {
            _processor = Client.CreateProcessor(QueueName);
        }
        
        public Task Subscribe(Func<ItemDto> handler)
        {
            throw new NotImplementedException();
        }
    }
}
