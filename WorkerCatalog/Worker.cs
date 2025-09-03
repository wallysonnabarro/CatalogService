using WorkerCatalog.Servico;

namespace WorkerCatalog
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRabbitMqClient _rabbitMqClient;

        public Worker(ILogger<Worker> logger, IRabbitMqClient rabbitMqClient)
        {
            _logger = logger;
            _rabbitMqClient = rabbitMqClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Conectar uma vez e manter conectado
                await _rabbitMqClient.ExecuteAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no Worker");
                throw;
            }
        }
    }
}
