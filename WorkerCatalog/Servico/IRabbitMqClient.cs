namespace WorkerCatalog.Servico
{
    public interface IRabbitMqClient
    {
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
