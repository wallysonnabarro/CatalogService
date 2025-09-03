namespace WorkerCatalog.Models
{
    public class ProdutoAtualizarSituacao
    {
        public Guid Id { get; set; }
        public required string Status { get; set; }
    }
}
