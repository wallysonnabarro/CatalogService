using CatalogService.Data;
using CatalogService.Services;

namespace OrderService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddScoped<ICorrelationLogger, CorrelationLogger>();
            // Registra o serviço de logging com Correlation ID
            Services.AddScoped<ICorrelationLogger, CorrelationLogger>();
            Services.AddScoped<IProdutosRepository, ProdutosRepository>();

            return Services;
        }
    }
}
