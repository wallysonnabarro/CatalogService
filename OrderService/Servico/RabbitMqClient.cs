using OrderService.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.Servico
{
    public class RabbitMqClient : IRabbitMqClient
    {
        private readonly IConfiguration _config;

        public RabbitMqClient(IConfiguration config)
        {
            _config = config;
        }

        public async Task PublicarAtualizarQuantidadeProdutosAsync(List<Produto> lista)
        {
            try
            {

                var factory = new ConnectionFactory
                {
                    HostName = _config["RabbitMQ:Host"]!,
                    Port = Int32.Parse(_config["RabbitMQ:Port"]!),
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

                throw;
            }
        }
    }
}
