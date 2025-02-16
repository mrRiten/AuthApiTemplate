using EmailApiTemplate.Models;

namespace EmailApiTemplate.Services
{
    public class RegisterBackgroundService(ILogger<RegisterBackgroundService> logger,
        ConsumerRabbitFactory<UserLogin> consumerRabbitFactory) : BackgroundService
    {
        private readonly ILogger<RegisterBackgroundService> _logger = logger;
        private readonly ConsumerRabbitFactory<UserLogin> _consumerRabbitFactory = consumerRabbitFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"RegisterBackgroundService is starting.");

            var consumer = await _consumerRabbitFactory.CreateService();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("RegisterBackgroundService is running at: {time}", DateTimeOffset.Now);

                await consumer.StartListeningAsync(async (message) =>
                {
                    _logger.LogInformation($"Received message with CorrelationId {message.CorrelationId}");
                    _logger.LogInformation($"Message data: {message.Username} - {message.Password}");
                });

                await Task.Delay(1000);
            }

            _logger.LogInformation("RegisterBackgroundService is stopping.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MyBackgroundService is stopping.");
            return base.StopAsync(cancellationToken);
        }

    }
}
