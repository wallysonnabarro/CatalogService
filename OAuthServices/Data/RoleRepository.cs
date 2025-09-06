using Microsoft.EntityFrameworkCore;
using OAuthServices.Data.Generico;
using OAuthServices.Models;

namespace OAuthServices.Data
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ContextDb context) : base(context)
        {
        }

        public async Task AddRole(Role role)
        {
            await AddAsync(role);
            await Savar();
        }

        public async Task DeleteRole(Guid id)
        {
            Remove(await GetRoleById(id));
            await Savar();
        }

        public async Task<Role> GetRoleById(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<Role> GetRoleByName(string roleName)
        {
            var roles = await _context.Roles.FirstOrDefaultAsync(x => x.Nome.Equals(roleName));

            if (roles == null)
            {
                var novo = new Role
                {
                    Nome = roleName,
                    Status = 1
                };

                await AddRole(novo);
                return novo;
            }

            return roles;
        }

        public async Task<bool> RoleExists(string roleName)
        {
            return await ExistsAsync(x => x.Nome.Equals(roleName));
        }

        public async Task UpdateRole(Role role)
        {
            Update(role);
            await Savar();
        }

        private async Task Savar()
        {
            await SaveChangesAsync();
        }

        public async Task<List<Role>> GetRolesByUserId(Guid userId)
        {
            return await _context.Usuarios
                .Where(u => u.Id == userId)
                .Select(u => u.Roles)
                .ToListAsync();
        }
    }
}
