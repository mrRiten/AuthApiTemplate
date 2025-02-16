using System.Text;
using EmailApiTemplate.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EmailApiTemplate.Services
{
    public class ConsumerService
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly ILogger<ConsumerService> _logger;
        private readonly ConnectionFactory _factory;
        private AsyncEventingBasicConsumer? _consumer;

        public ConsumerService(ILogger<ConsumerService> logger)
        {
            _logger = logger;
            _factory = new ConnectionFactory() { HostName = "localhost" };
        }

        public async Task ConnectAsync()
        {
            try
            {
                _connection = await _factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(queue: "registration",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ broker.");
            }
        }

        public async Task StartListening()
        {
            if (_channel == null || _connection == null)
            {
                _logger.LogError("Connection to RabbitMQ is not established.");
                return;
            }

            _consumer = new AsyncEventingBasicConsumer(_channel);

            _consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"Received message: {message}");

                try
                {
                    var user = JsonConvert.DeserializeObject<UserLogin>(message);
                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);

                    HandleMessage(user);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to deserialize message.");
                }
            };

            await _channel.BasicConsumeAsync(queue: "registration", 
                                            autoAck: false, 
                                            consumer: _consumer);
        }

        private void HandleMessage(UserLogin user)
        {
            // Здесь можно добавить обработку полученного сообщения
            _logger.LogInformation($"Processing user: {user.Username}");
        }

        public async Task DisposeAsync()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();

            _logger.LogInformation("Consumer connection closed.");
        }
    }
}
