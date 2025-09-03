using OrderService.Models;

namespace OrderService.Servico
{
    public interface IRabbitMqClient
    {
        Task PublicarAtualizarQuantidadeProdutosAsync(List<Produto> lista);
    }
}
