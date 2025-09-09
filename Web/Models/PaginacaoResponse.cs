namespace Web.Models
{
    public class PaginacaoResponse<T>
    {
        public int Count { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<T> Dados { get; set; } = new();
    }

    // Para compatibilidade com código existente
    public class PaginacaoResponse : PaginacaoResponse<ProdutoViewModel>
    {
    }
}
