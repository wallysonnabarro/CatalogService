using System.Text.Json;
using WorkerOrdem.Data;

namespace WorkerOrdem.Services
{
    public class ProcessarEvento : IProcessarEvento
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProcessarEvento> _logger;

        public ProcessarEvento(IServiceScopeFactory scopeFactory, ILogger<ProcessarEvento> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void ProcessarMensagemEvento(string m)
        {
            _logger.LogInformation($"Iniciando processamento de mensagem do RabbitMQ - event {m}");

            try
            {
                using var scope = _scopeFactory.CreateScope();

                var _Repository = scope.ServiceProvider.GetRequiredService<IOrdemRepository>();

                var message = JsonSerializer.Deserialize<string>(m);

                if (message != null && message.Any())
                {
                    _logger.LogInformation($"Processando o cancelamento da ordem id = {message}");

                    _Repository.CancelarOrdem(message).GetAwaiter().GetResult();

                    _logger.LogInformation("Cancelamento da ordem concluída com sucesso");
                }
                else
                {
                    _logger.LogWarning($"Mensagem inválida recebida: {m}");

                    throw new Exception("Mensagem inválida");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem do RabbitMQ: {Mensagem}", m);
                throw;
            }
        }
    }
}
