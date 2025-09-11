using WorkerOrdem.Services;

namespace WorkerOrdem
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRabbitMqReverterOrdem _rabbitMqOrdem;

        public Worker(ILogger<Worker> logger, IRabbitMqReverterOrdem rabbitMqOrdem)
        {
            _logger = logger;
            _rabbitMqOrdem = rabbitMqOrdem;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker iniciado - {Timestamp}", DateTime.UtcNow);

            try
            {
                _logger.LogInformation("Conectando ao RabbitMQ...");

                // Conectar uma vez e manter conectado
                await _rabbitMqOrdem.ExecuteAsync(stoppingToken);

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
