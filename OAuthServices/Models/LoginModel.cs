using System.ComponentModel.DataAnnotations;

namespace OAuthServices.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "O campo E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        public required string Senha { get; set; }
        public bool RememberMe { get; set; }
    }
}
