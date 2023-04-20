using Azure.Messaging.ServiceBus;
using Infrastructure.ServiceBus.DTO;
using System.Text.Json;
using Infrastructure.ServiceBus.Interfaces;

namespace Infrastructure.ServiceBus
{
    public class MessagePublisher : MessagingClient, IMessagePublisher
    {
        private readonly ServiceBusSender _sender;

        public MessagePublisher()
        {
            _sender = Client.CreateSender(QueueName);
        }

        public async Task Send(ItemDto dto)
        {
            var payload = JsonSerializer.Serialize(dto);
            var message = new ServiceBusMessage(payload);

            await _sender.SendMessageAsync(message);
        }
    }
}
