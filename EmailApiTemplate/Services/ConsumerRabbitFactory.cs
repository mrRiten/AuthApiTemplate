using EmailApiTemplate.Models;

namespace EmailApiTemplate.Services
{
    public class ConsumerRabbitFactory<TMessage>(string hostName, string queueName) where TMessage : IBrokerMessage
    {
        private string _hostName { get; set; } = hostName;
        private string _queueName { get; set; } = queueName;

        public async Task<ConsumerRabbitService<TMessage>> CreateService()
        {
            var consumerService = new ConsumerRabbitService<TMessage>(_hostName, _queueName);
            await consumerService.StartConsume();
            
            var channel = consumerService.Channel;

            if (channel == null) { await Task.CompletedTask; }

            await channel.QueueDeclareAsync(queue: _queueName,
                                            durable: true,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);
            return consumerService;
        }
    }
}
