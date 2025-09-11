
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace WorkerCatalog.Services
{
    public class RabbitMqReverterOrdem : IRabbitMqReverterOrdem
    {
        private readonly ILogger<RabbitMqClient> _logger;
        private readonly IConfiguration _config;

        public RabbitMqReverterOrdem(ILogger<RabbitMqClient> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task ReverterOrdemAsync(Guid ordemId, string correlationId)
        {
            try
            {
                _logger.LogInformation($"Iniciando publicar a reverção da ordem - event {ordemId.ToString()}");

                var factory = new ConnectionFactory
                {
                    HostName = _config["RabbitMQ:Host"]!,
                    Port = int.Parse(_config["RabbitMQ:Port"]!),
                    UserName = _config["RabbitMQ:User"]!,
                    Password = _config["RabbitMQ:Password"]!
                };

                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                var arguments = new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "dead_letters" },
                    { "x-dead-letter-routing-key", "ordem.dlx" },
                    { "x-max-length", 100 }
                };

                await channel.ExchangeDeclareAsync(
                    exchange: "ordem_exchange",
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false);

                await channel.QueueDeclareAsync(
                    queue: "ordem",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments!);

                await channel.QueueBindAsync(
                    queue: "ordem",
                    exchange: "ordem_exchange",
                    routingKey: "ordem");

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ordemId));

                var props = new BasicProperties
                {
                    MessageId = ordemId.ToString(),
                    CorrelationId = correlationId,
                    Headers = new Dictionary<string, object?>
                    {
                        { "x-event-type", "AtualizarQuantidadeProdutos" },
                        { "x-created-at", DateTime.UtcNow.ToString("o") },
                        { "x-item-count", ordemId.ToString()}
                    },
                    Persistent = true
                };

                await channel.BasicPublishAsync(
                    exchange: "ordem_exchange",
                    routingKey: "ordem",
                    mandatory: true,
                    basicProperties: props,
                    body: body);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao publicar a reverção da ordem - event", ordemId.ToString());
                throw;
            }
        }
    }
}
