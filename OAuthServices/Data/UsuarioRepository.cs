using Microsoft.EntityFrameworkCore;
using OAuthServices.Data.Generico;
using OAuthServices.Models;

namespace OAuthServices.Data
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ContextDb context) : base(context)
        {
        }

        public async Task Atualizar(Usuario user)
        {
            Update(user);
            await SaveChangesAsync();
        }

        public async Task<Usuario> GetUserByEmail(string email)
        {
            return await FirstWithIncludsAsync(
                x => x.Email.Equals(email),
                include: q => q.Include(x => x.Roles));
        }

        public async Task<bool> GetUserByEmailAny(string email)
        {
            var existe = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email.Equals(email));

            if(existe == null) return false;

            return true;
        }

        public async Task<Guid> Novo(Usuario usuario)
        {
            try
            {
                var role = await _context.Roles.FirstAsync(x => x.Nome == "GESTOR");

                if (role == null)
                {
                    throw new Exception("Role 'GESTOR' não encontrada.");
                }

                usuario.RoleId = role.Id; 

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                return usuario.Id; 
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task UpdateAcessLock(Guid id)
        {
            var usuario = await FirstWithIncludsAsync(
                x => x.Id == id);

            if (usuario != null)
            {
                usuario.AccessFailedCount++;

                if (usuario.AccessFailedCount > 3)
                {
                    usuario.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(15);
                    usuario.LockoutEnabled = true;
                }

                await SaveChangesAsync();
            }
        }

        public async Task<bool> DesbloquearUsuario(string email)
        {
            try
            {
                var usuario = await ByExpressionData(x => x.Email.Equals(email));
                if (usuario != null)
                {
                    usuario.LockoutEnd = null;
                    usuario.LockoutEnabled = false;
                    await SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
