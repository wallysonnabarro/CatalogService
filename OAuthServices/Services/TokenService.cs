using Microsoft.IdentityModel.Tokens;
using OAuthServices.Data;
using OAuthServices.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OAuthServices.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenRepository _token;

        public TokenService(IConfiguration configuration, ITokenRepository token)
        {
            _configuration = configuration;
            _token = token;
        }

        public async Task<string> CreateAccessToken(Guid? userId, IEnumerable<Role> roles, TimeSpan? ttl = null)
        {
            return await Task.Run(() =>
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, userId.ToString()!) };
                claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r.Id.ToString())));

                var token = new JwtSecurityToken(_configuration["JwtSettings:Issuer"]!, _configuration["JwtSettings:Audience"]!, claims,
                    expires: DateTime.UtcNow.Add(ttl ?? TimeSpan.FromMinutes(5)),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            });
        }

        public async Task<(string token, string jti, DateTime exp)> CreateRefreshToken(Guid? userId, TimeSpan? ttl = null)
        {
            return await Task.Run(async () =>
            {
                var jti = Guid.NewGuid().ToString("N");
                var exp = DateTime.UtcNow.Add(ttl ?? TimeSpan.FromDays(7));

                if (userId == null) throw new ArgumentNullException(nameof(userId));

                await _token.SaveRefreshTokenAsync(jti, (Guid)userId, exp);

                return (Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)), jti, exp);
            });
        }

        public async Task<(bool existe, Guid? userId)> TryUseRefresh(Guid id)
        {
            Guid? userId = null;
            var refresh = await _token.TryGetValue(id);

            if (refresh == null) return (false, userId);

            if (refresh.Expiration < DateTime.UtcNow)
            {
                _token.Remove(id); return (false, userId);
            }

            userId = refresh.UserId;

            return (false, userId);
        }

        public async Task<bool> ValidarToken(string token)
        {
            return await Task.Run(() => { 
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration["JwtSettings:Issuer"],
                        ValidAudience = _configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }
    }

}
