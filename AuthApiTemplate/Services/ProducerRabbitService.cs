using System.Text;
using AuthApiTemplate.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace AuthApiTemplate.Services
{
    public class ProducerRabbitService<TMessage> where TMessage : IBrokerMessage
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;
        private IConnection _connection;
        private IChannel _channel;

        public ProducerRabbitService(string hostName, string queueName)
        {
            _factory = new ConnectionFactory { HostName = hostName };
            _queueName = queueName;
        }

        public async Task StartProduce()
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task SendMessageAsync(TMessage message)
        {
            await _channel.QueueDeclareAsync(queue: _queueName,
                                            durable: true,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

            var messageBody = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageBody);

            await _channel.BasicPublishAsync(exchange: "",
                                  routingKey: _queueName,
                                  body: body);
        }
    }
}
