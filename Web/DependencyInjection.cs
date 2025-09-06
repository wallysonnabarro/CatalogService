using Web.Services;

namespace Web
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddScoped<ICorrelationLogger, CorrelationLogger>();
            Services.AddScoped<ILoginServices, LoginServices>();
            Services.AddScoped<IUsuarioServices, UsuarioServices>();

            return Services;
        }
    }
}
