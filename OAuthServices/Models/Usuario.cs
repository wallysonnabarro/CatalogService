using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace OAuthServices.Models
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        [ProtectedPersonalData]
        public virtual required string Email { get; set; }
        [PersonalData]
        public virtual bool EmailConfirmed { get; set; }
        public virtual required string PasswordHash { get; set; }
        [JsonIgnore]
        public required string Salt { get; set; }
        public int? Tentativas { get; set; }
        public virtual required string SecurityStamp { get; set; }
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        public virtual DateTimeOffset? LockoutEnd { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual int AccessFailedCount { get; set; }
        public DateTime DtRegistro { get; set; }
        public Guid RoleId { get; set; }
        public required Role Roles { get; set; }
    }
}
