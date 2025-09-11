namespace WorkerCatalog.Services
{
    public interface IRabbitMqReverterOrdem
    {
        Task ReverterOrdemAsync(Guid ordemId, string correlationId);
    }
}
