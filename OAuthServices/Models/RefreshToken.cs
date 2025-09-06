namespace OAuthServices.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } // Chave primária no banco

        public string Token { get; set; } = string.Empty; // Refresh Token gerado
        public Guid UserId { get; set; } = Guid.NewGuid(); // Relacionado ao usuário (pode ser Guid ou int dependendo da tabela de usuários)

        public DateTime Expiration { get; set; } // Data de expiração

        public bool Revoked { get; set; } // Se foi revogado antes da expiração

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Quando foi criado
    }
}
