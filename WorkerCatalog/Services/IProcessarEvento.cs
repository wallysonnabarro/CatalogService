using static MassTransit.Monitoring.Performance.BuiltInCounters;

namespace WorkerCatalog.Services
{
    public interface IProcessarEvento
    {
        Task ProcessarMensagemEvento(string mensagem, string messageId, string correlationId);
    }
}
