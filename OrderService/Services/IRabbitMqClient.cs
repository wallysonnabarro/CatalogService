using OrderService.Models;

namespace OrderService.Services
{
    public interface IRabbitMqClient
    {
        Task PublicarAtualizarQuantidadeProdutosAsync(List<Produto> lista, Guid idOrdem);
    }
}
