using OrderService.Models;

namespace OrderService.Data
{
    public class OrdemServicoRepository : GenericRepository<OrdermServico>, IOrdemServicoRepository
    {
        public OrdemServicoRepository(ContextDb context) : base(context)
        {
            
        }

        public async Task<Guid> GravarOrdemServiço(OrdermServico orderm)
        {
            await AddAsync(orderm);
            return orderm.Id;
        }
    }
}
