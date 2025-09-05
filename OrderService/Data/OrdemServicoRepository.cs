using OrderService.Models;
using OrderService.Services;

namespace OrderService.Data
{
    public class OrdemServicoRepository : GenericRepository<OrdermServico>, IOrdemServicoRepository
    {
        private readonly ICorrelationLogger _logger;

        public OrdemServicoRepository(ContextDb context, ICorrelationLogger logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<Guid> GravarOrdemServiço(OrdermServico orderm)
        {
            try
            {
                _logger.LogInformation("Iniciando persistencia da ordem de serviço.", orderm.Produtos.Count);

                await AddAsync(orderm);
                return orderm.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao persistir a order de serviço", orderm.Produtos.Count);
                throw;
            }
        }
    }
}
