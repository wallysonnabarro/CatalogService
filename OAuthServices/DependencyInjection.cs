using OAuthServices.Aplicacao;
using OAuthServices.Data;
using OAuthServices.Services;

namespace OAuthServices
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddScoped<IJWTOAuth, JWTOAuth>();
            Services.AddScoped<ICorrelationLogger, CorrelationLogger>();
            Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            Services.AddScoped<IRoleRepository, RoleRepository>();
            Services.AddScoped<ITokenRepository, TokenRepository>();
            Services.AddScoped<ITokenService, TokenService>();
            Services.AddScoped<ICadastroUsuarioUseCase, CadastroUsuarioUseCase>();
            Services.AddScoped<IValidarEmailUseCase, ValidarEmailUseCase>();
            Services.AddScoped<ICodeGenerate, CodeGenerate>();
            Services.AddScoped<IGenerateHashs, GenerateHashs>();

            return Services;
        }
    }
}
