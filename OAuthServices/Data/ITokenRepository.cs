using OAuthServices.Models;

namespace OAuthServices.Data
{
    public interface ITokenRepository
    {
        Task SaveRefreshTokenAsync(string token, Guid userId, DateTime expiration);
        Task<RefreshToken> TryGetValue(Guid id);
        void Remove(Guid id);
    }
}
