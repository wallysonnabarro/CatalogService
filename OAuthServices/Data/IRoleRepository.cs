using OAuthServices.Data.Generico;
using OAuthServices.Models;

namespace OAuthServices.Data
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetRoleByName(string roleName);
        Task<bool> RoleExists(string roleName);
        Task AddRole(Role role);
        Task UpdateRole(Role role);
        Task DeleteRole(Guid id);
        Task<Role> GetRoleById(Guid id);
        Task<List<Role>> GetRolesByUserId(Guid userId);
    }
}
