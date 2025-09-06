using Microsoft.EntityFrameworkCore;
using OAuthServices.Data.Generico;
using OAuthServices.Models;

namespace OAuthServices.Data
{
    public class TokenRepository : GenericRepository<RefreshToken>, ITokenRepository
    {
        public TokenRepository(ContextDb context) : base(context)
        {

        }

        public void Remove(Guid id)
        {
            var token = _context.RefreshTokens.FirstOrDefault(t => t.Id == id);
            if (token != null)
            {
                _context.RefreshTokens.Remove(token);
                _context.SaveChanges();
            }
        }

        public async Task SaveRefreshTokenAsync(string token, Guid userId, DateTime expiration)
        {
            var refreshToken = new RefreshToken
            {
                Token = token,
                UserId = userId,
                Expiration = expiration
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> TryGetValue(Guid id)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
