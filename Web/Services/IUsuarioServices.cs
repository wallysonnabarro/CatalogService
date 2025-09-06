using Web.Models;

namespace Web.Services
{
    public interface IUsuarioServices
    {
        Task<bool> VerificarEmailExiste(string email);
        Task NovoUsuario(NovoUsuarioModel model);
    }
}
