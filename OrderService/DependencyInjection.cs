
using OrderService.Aplicacao;
using OrderService.Data;
using OrderService.Services;

namespace OrderService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddScoped<ICorrelationLogger, CorrelationLogger>();
            Services.AddScoped<IOrdemServicoUseCase, OrdemServicoUseCase>();
            Services.AddScoped<IOrdemServicoServices, OrdemServicoServices>();
            Services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
            Services.AddScoped<IOrderMappingService, OrderMappingService>();
            Services.AddScoped<IRabbitMqClient, RabbitMqClient>();
            return Services;
        }
    }
}
