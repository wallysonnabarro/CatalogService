namespace WorkerCatalog.Services
{
    public interface IRabbitMqClient
    {
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
