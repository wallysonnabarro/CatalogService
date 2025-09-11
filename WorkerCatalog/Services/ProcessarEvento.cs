using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WorkerCatalog.Data;
using WorkerCatalog.Models;

namespace WorkerCatalog.Services
{
    public class ProcessarEvento : IProcessarEvento
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProcessarEvento> _logger;
        private readonly IRabbitMqReverterOrdem _reverterOrdem;

        public ProcessarEvento(IServiceScopeFactory scopeFactory, ILogger<ProcessarEvento> logger, IRabbitMqReverterOrdem reverterOrdem)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _reverterOrdem = reverterOrdem;
        }

        public async Task ProcessarMensagemEvento(string m, string messageId, string correlationId)
        {
            _logger.LogInformation("Iniciando processamento de mensagem do RabbitMQ");

            try
            {
                using var scope = _scopeFactory.CreateScope();

                var _produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutosRepository>();

                var message = JsonSerializer.Deserialize<List<ProdutoAtualizarQuantidade>>(m);

                if (message != null && message.Any())
                {
                    _logger.LogInformation("Processando atualização de {Quantidade} produtos", message.Count);

                    _produtoRepository.AtualizarQuantidadeProdutos(message).GetAwaiter().GetResult();

                    _logger.LogInformation("Atualização de produtos concluída com sucesso");
                }
                else
                {
                    _logger.LogWarning("Mensagem inválida recebida: {Mensagem}", m);

                    await _reverterOrdem.ReverterOrdemAsync(Guid.Parse(messageId), correlationId);

                    throw new Exception("Mensagem inválida");
                }
            }
            catch (Exception ex)
            {
                await _reverterOrdem.ReverterOrdemAsync(Guid.Parse(messageId), correlationId);
                _logger.LogError(ex, "Erro ao processar mensagem do RabbitMQ: {Mensagem}", m);
                throw;
            }
        }
    }
}
