using WorkerCatalog.Services;

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
            _logger.LogInformation("Worker iniciado - {Timestamp}", DateTime.UtcNow);
            
            try
            {
                _logger.LogInformation("Conectando ao RabbitMQ...");
                
                // Conectar uma vez e manter conectado
                await _rabbitMqClient.ExecuteAsync(stoppingToken);
                
                _logger.LogInformation("Worker finalizado normalmente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro crítico no Worker - {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}
