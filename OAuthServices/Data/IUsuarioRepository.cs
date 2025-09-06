using OAuthServices.Data.Generico;
using OAuthServices.Models;

namespace OAuthServices.Data
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<Usuario> GetUserByEmail(string email);
        Task<bool> GetUserByEmailAny(string email);
        Task<Guid> Novo(Usuario usuario);
        Task UpdateAcessLock(Guid id);
        Task<bool> DesbloquearUsuario(string email);
    }
}
