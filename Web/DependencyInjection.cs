using Web.Services;
using Web.Filter;

namespace Web
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddScoped<ICorrelationLogger, CorrelationLogger>();
            Services.AddScoped<ILoginServices, LoginServices>();
            Services.AddScoped<IUsuarioServices, UsuarioServices>();
            Services.AddScoped<IProdutoServices, ProdutoServices>();
            Services.AddScoped<IOrderService, OrderService>();
            Services.AddScoped<ValidarTokenFilter>();

            return Services;
        }
    }
}
