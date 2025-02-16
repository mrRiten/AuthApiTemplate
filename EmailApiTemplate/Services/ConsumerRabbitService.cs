using System.Text;
using EmailApiTemplate.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmailApiTemplate.Services
{
    public class ConsumerRabbitService<TMessage> where TMessage : IBrokerMessage
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;
        private IConnection? _connection;
        private IChannel? _channel;

        public IChannel? Channel { get { return _channel; } }

        public ConsumerRabbitService(string hostName, string queueName)
        {
            _factory = new ConnectionFactory { HostName = hostName };
            _queueName = queueName;
        }

        public async Task StartConsume()
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task StartListeningAsync(Func<TMessage, Task> onMessageReceived)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);

                try
                {
                    var message = JsonConvert.DeserializeObject<TMessage>(messageJson);

                    if (message != null)
                    {
                        await onMessageReceived(message);
                        await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Failed to deserialize message: {ex.Message}");
                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
            };

            await _channel.BasicConsumeAsync(queue: _queueName,
                                            autoAck: false,
                                            consumer: consumer);
        }
    }
}
