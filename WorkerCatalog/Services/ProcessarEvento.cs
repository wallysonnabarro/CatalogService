using System.Text;
using System.Text.Json;
using WorkerCatalog.Data;
using WorkerCatalog.Models;

namespace WorkerCatalog.Services
{
    public class ProcessarEvento : IProcessarEvento
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ProcessarEvento(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void ProcessarMensagemEvento(string m)
        {
            using var scope = _scopeFactory.CreateScope();

            var _produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutosRepository>();

            var message = JsonSerializer.Deserialize<List<ProdutoAtualizarQuantidade>>(m);

            if (message != null && message.Any())
            {
                _produtoRepository.AtualizarQuantidadeProdutos(message).GetAwaiter().GetResult();
            }
            else
            {
                throw new Exception("Mensagem inválida");
            }
        }
    }
}
