using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace WorkerCatalog.Servico
{
    public class RabbitMqClient : IRabbitMqClient
    {

        private readonly IConfiguration _config;
        private readonly IProcessarEvento _processarEvento;
        private IConnection? _connection;
        private IChannel? _channel;
        private bool _isConnected = false;

        public RabbitMqClient(IConfiguration config, IProcessarEvento processarEvento)
        {
            _config = config;
            _processarEvento = processarEvento;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_isConnected)
            {
                return; // Já está conectado e processando
            }

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _config["RabbitMQ:Host"]!,
                    Port = Int32.Parse(_config["RabbitMQ:Port"]!),
                    UserName = _config["RabbitMQ:User"]!,
                    Password = _config["RabbitMQ:Password"]!
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                _isConnected = true;

                var arguments = new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "dead_letters" },
                    { "x-dead-letter-routing-key", "catalog.dlx" },
                    { "x-max-length", 100 }
                };

                await _channel.QueueDeclareAsync(
                    queue: "catalog",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments!);

                await _channel.BasicQosAsync(0, 1, false);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        byte[] body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        if (!string.IsNullOrEmpty(message))
                        {
                            _processarEvento.ProcessarMensagemEvento(message);

                            await _channel.BasicAckAsync(ea.DeliveryTag, false);
                        }
                        else
                        {
                            await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                        }
                    }
                    catch (Exception)
                    {
                        // Rejeitar mensagem com requeue para tentar novamente
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                };

                // IMPORTANTE: autoAck deve ser false para controle manual
                await _channel.BasicConsumeAsync("catalog", autoAck: false, consumer: consumer);

                // Manter a conexão ativa até ser cancelada
                while (_connection.IsOpen && !stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(50000, stoppingToken);
                }
            }
            catch (Exception)
            {
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
