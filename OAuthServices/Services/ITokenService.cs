using OAuthServices.Models;

namespace OAuthServices.Services
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(Guid? userId, IEnumerable<Role> roles, TimeSpan? ttl = null);
        Task<(string token, string jti, DateTime exp)> CreateRefreshToken(Guid? userId, TimeSpan? ttl = null);
        Task<(bool existe, Guid? userId)> TryUseRefresh(Guid id);
        Task<bool> ValidarToken(string token);

    }
}