using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace OAuthServices.Models
{
    public class UsuarioLoginModel
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public virtual required string Email { get; set; }
        public required Guid RoleId { get; set; }
        public int? Tentativas { get; set; }
        public virtual required string PasswordHash { get; set; }
        [JsonIgnore]
        public required string Salt { get; set; }
        [PersonalData]
        public virtual bool EmailConfirmed { get; set; }
    }
}
