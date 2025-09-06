namespace OAuthServices.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public int Status { get; set; }

        public List<Usuario> Usuarios { get; set; }
    }
}