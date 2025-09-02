using CatalogService.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CatalogService.Aplicacao
{
    public class JWTOAuth : IJWTOAuth
    {
        private readonly IConfiguration _configuration;

        public JWTOAuth(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetToken()
        {
            return await Task.Run(() =>
            {
                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];
                var secureKey = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);
                var key = new SymmetricSecurityKey(secureKey);
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, "Catalog")
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    Audience = audience,
                    Issuer = issuer,
                    SigningCredentials = credentials
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            });
        }
    }
}
