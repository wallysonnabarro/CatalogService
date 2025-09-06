using OAuthServices.Data.Generico;
using OAuthServices.Models;
using OAuthServices.Models.Resultados;
using System.Security.Cryptography;

namespace OAuthServices.Data
{
    public class AuthenticationRepository : GenericRepository<Usuario>, IAuthenticationRepository
    {
        private readonly IConfiguration _configuration;

        public AuthenticationRepository(ContextDb context, IConfiguration configuration) : base(context)
        {
            _configuration = configuration;
        }

        public async Task<Result<Guid>> AuthenticateEmail(LoginModel model)
        {
            try
            {
                if (!await ExistsAsync(x => x.Email.Equals(model.Email))) return Result<Guid>.Failed(new Errors { Code = "404", Description = "Usuário não encontrado" });

                var dados = await ByExpressionData(x => x.Email.Equals(model.Email));

                var identidade = await VerifyPassword(model.Senha, dados.PasswordHash, dados.Salt);

                if(!identidade.Succeeded)
                {
                    return Result<Guid>.Failed(identidade.Error!);
                }

                return Result<Guid>.Sucesso(dados.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failed(new Errors { Description = e.Message });
            }
        }

        private async Task<Identidade> VerifyPassword(string senha, string hash, string salt)
        {
            return await Task.Run(() =>
            {
                byte[] saltBytes = Convert.FromBase64String(salt);

                using Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(senha, saltBytes, Convert.ToInt32(_configuration["SETTING:ITERATIONS"]), HashAlgorithmName.SHA256);

                byte[] computedHash = pbkdf2.GetBytes(Convert.ToInt32(_configuration["SETTING:BASE_64"]));

                string computedHashBase64 = Convert.ToBase64String(computedHash);

                if (hash.Equals(computedHashBase64)) return Identidade.Success;
                else return Identidade.Failed(new Errors { Code = "403", Description = "Token Invalid" });
            });
        }
    }
}
