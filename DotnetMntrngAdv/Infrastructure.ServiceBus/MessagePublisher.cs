using Azure.Messaging.ServiceBus;
using Infrastructure.ServiceBus.DTO;
using System.Text.Json;
using Infrastructure.ServiceBus.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ServiceBus
{
    public class MessagePublisher : MessagingClient, IMessagePublisher
    {
        private readonly ServiceBusSender _sender;
        private readonly ILogger<MessagePublisher> _logger;

        public MessagePublisher(ILogger<MessagePublisher> logger)
        {
            _logger = logger;
            _sender = Client.CreateSender(QueueName);
        }

        public async Task Send(ItemDto dto)
        {
            var payload = JsonSerializer.Serialize(dto);
            var message = new ServiceBusMessage(payload);
            
            _logger.LogInformation($"Sending a new message [id = {message.MessageId}, subject = {message.Subject}, correlationId = {message.CorrelationId}] with payload [payload = {payload}] to ServiceBus");
            await _sender.SendMessageAsync(message);
        }
    }
}
