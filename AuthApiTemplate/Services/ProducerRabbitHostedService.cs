
using AuthApiTemplate.Models;

namespace AuthApiTemplate.Services
{
    public class ProducerRabbitHostedService<TMessage>(ProducerRabbitService<TMessage> producer) 
        : IHostedService 
        where TMessage : IBrokerMessage
    {
        private readonly ProducerRabbitService<TMessage> _producer = producer;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _producer.StartProduce();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
