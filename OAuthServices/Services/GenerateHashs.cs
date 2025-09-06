using System.Security.Cryptography;

namespace OAuthServices.Services
{
    public class GenerateHashs(IConfiguration configuration) : IGenerateHashs
    {

        public async Task<byte[]> GeneratePasswordHash(string password, byte[] salt)
        {
            return await Task.Run(() =>
            {
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Convert.ToInt32(configuration["SETTING:ITERATIONS"]), HashAlgorithmName.SHA256))
                {
                    return pbkdf2.GetBytes(Convert.ToInt32(configuration["SETTING:BASE_64"]));
                }
            });
        }

        public async Task<byte[]> GenerateSalt()
        {
            return await Task.Run(() =>
            {
                byte[] salt = new byte[Convert.ToInt32(configuration["SETTING:SALTVALUE"])];

                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                return salt;
            });
        }
    }
}
