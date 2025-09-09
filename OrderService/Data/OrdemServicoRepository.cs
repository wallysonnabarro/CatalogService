using Microsoft.EntityFrameworkCore;
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

        public async Task<List<OrdermServico>> ListarOrdensAsync()
        {
            try
            {
                _logger.LogInformation("Buscando lista de ordens de serviço");

                var ordens = await GetAllAsync();
                return ordens.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar lista de ordens de serviço");
                throw;
            }
        }

        public async Task<OrdermServico?> ObterOrdemPorIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Buscando ordem por ID: {OrderId}", id);

                var ordem = await FirstWithIncludsAsync(
                    predicate: x => x.Id == id,
                    include: i => i.Include(p => p.Produtos));
                return ordem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ordem por ID: {OrderId}", id);
                throw;
            }
        }

        public async Task<OrderPageResponse<OrdermServico>> ListarOrdensPaginadasAsync(OrderPageRequest request)
        {
            try
            {
                _logger.LogInformation("Buscando ordens paginadas. Página: {Page}, Tamanho: {PageSize}", request.Page, request.PageSize);

                var query = _context.OrdermServicos.Include(o => o.Produtos).AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(request.Status))
                {
                    query = query.Where(o => o.Status == request.Status);
                }

                // Ordenar por data mais recente
                query = query.OrderByDescending(o => o.DataHoraRegistro);

                // Contar total de registros
                var totalCount = await query.CountAsync();

                // Aplicar paginação
                var ordens = await query.Skip(request.Skip).Take(request.PageSize).ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                var response = new OrderPageResponse<OrdermServico>
                {
                    Dados = ordens,
                    Count = totalCount,
                    PageIndex = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = totalPages
                };

                _logger.LogInformation("Ordens paginadas obtidas com sucesso. Total: {Total}, Página: {Page}", totalCount, request.Page);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ordens paginadas");
                throw;
            }
        }
    }
}
