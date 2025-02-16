using AuthApiTemplate.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace AuthApiTemplate.Services
{
    public class ProducerService
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly ILogger<ProducerService> _logger;
        private readonly ConnectionFactory _factory;

        public ProducerService(ILogger<ProducerService> logger)
        {
            _logger = logger;
            _factory = new ConnectionFactory() { HostName = "localhost" };
        }

        public async Task Connection()
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
                _logger.LogInformation(ex.ToString());
            }
        }

        private bool CheckConnection()
        {
            return _channel != null || _connection != null;
        }

        public async Task RegisterUser(UserLogin user)
        {
            if (!CheckConnection())
            {
                await Connection();
            }

            var message = JsonConvert.SerializeObject(user);
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync("", "registration", body);
        }

        public async Task Dispose()
        {
            await _channel?.CloseAsync();
            await _connection?.CloseAsync();
        }
    }
}
