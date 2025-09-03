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
                    HostName = _config["RabbitMqHost"]!,
                    Port = 5672,
                    UserName = _config["RabbitUsername"]!,
                    Password = _config["RabbitPassword"]!
                };

                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(
                    queue: "catalog",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

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
