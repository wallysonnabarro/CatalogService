using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace WorkerCatalog.Services
{
    public class RabbitMqClient : IRabbitMqClient
    {

        private readonly IConfiguration _config;
        private readonly IProcessarEvento _processarEvento;
        private readonly ILogger<RabbitMqClient> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private bool _isConnected = false;

        public RabbitMqClient(IConfiguration config, IProcessarEvento processarEvento, ILogger<RabbitMqClient> logger)
        {
            _config = config;
            _processarEvento = processarEvento;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_isConnected)
            {
                _logger.LogInformation("RabbitMQ já está conectado e processando");
                return; // Já está conectado e processando
            }

            try
            {
                _logger.LogInformation("Iniciando conexão com RabbitMQ - Host: {Host}:{Port}", 
                    _config["RabbitMQ:Host"], _config["RabbitMQ:Port"]);

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

                _logger.LogInformation("Conexão com RabbitMQ estabelecida com sucesso");

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

                _logger.LogInformation("Fila 'catalog' configurada e aguardando mensagens");

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        byte[] body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        _logger.LogInformation("Mensagem recebida do RabbitMQ - Tamanho: {Tamanho} bytes", body.Length);

                        if (!string.IsNullOrEmpty(message))
                        {
                            _processarEvento.ProcessarMensagemEvento(message);
                            await _channel.BasicAckAsync(ea.DeliveryTag, false);
                            
                            _logger.LogInformation("Mensagem processada com sucesso");
                        }
                        else
                        {
                            _logger.LogWarning("Mensagem vazia recebida, rejeitando");
                            await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar mensagem do RabbitMQ, tentando novamente");
                        // Rejeitar mensagem com requeue para tentar novamente
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                };

                // IMPORTANTE: autoAck deve ser false para controle manual
                await _channel.BasicConsumeAsync("catalog", autoAck: false, consumer: consumer);

                _logger.LogInformation("Consumer iniciado e aguardando mensagens na fila 'catalog'");

                // Manter a conexão ativa até ser cancelada
                while (_connection.IsOpen && !stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(50000, stoppingToken);
                }
                
                _logger.LogInformation("Conexão RabbitMQ finalizada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na conexão RabbitMQ, tentando reconectar em 5 segundos");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
