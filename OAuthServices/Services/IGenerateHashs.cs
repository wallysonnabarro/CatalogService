namespace OAuthServices.Services
{
    public interface IGenerateHashs
    {
        Task<byte[]> GenerateSalt();
        Task<byte[]> GeneratePasswordHash(string password, byte[] salt);
    }
}
