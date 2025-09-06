using ApiGateway.Services;

namespace ApiGateway
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection Services, IConfiguration configuration)
        {
            // Registra o serviço de logging com Correlation ID
            Services.AddScoped<ICorrelationLogger, CorrelationLogger>();

            return Services;
        }
    }
}
