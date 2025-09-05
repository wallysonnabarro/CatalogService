using OrderService.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.Services
{
    public class RabbitMqClient : IRabbitMqClient
    {
        private readonly IConfiguration _config;
        private readonly ICorrelationLogger _logger;

        public RabbitMqClient(IConfiguration config, ICorrelationLogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task PublicarAtualizarQuantidadeProdutosAsync(List<Produto> lista)
        {
            try
            {
                _logger.LogInformation("Iniciando publicação da lista de produtos - evento", lista.Count);

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
                    { "x-dead-letter-routing-key", "catalog.dlx" },
                    { "x-max-length", 100 }                        
                };

                await channel.QueueDeclareAsync(
                    queue: "catalog",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments!);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(lista));

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "catalog",
                    body: body);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao publicar a lista de produtos - event", lista.Count);
                throw;
            }
        }
    }
}
