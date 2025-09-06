using System.ComponentModel.DataAnnotations;

namespace OAuthServices.Models
{
    public class NovoUsuarioModel
    {
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; } 
        public required string ConfirmarSenha { get; set; }
    }
}
