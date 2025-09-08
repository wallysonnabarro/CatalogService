namespace Web.Models
{
    public class PaginacaoResponse
    {
        public int Count { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<ProdutoViewModel> Dados { get; set; } = new();
    }
}
