namespace WorkerOrdem.Services
{
    public interface IRabbitMqReverterOrdem
    {
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
