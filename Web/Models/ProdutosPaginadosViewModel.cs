namespace Web.Models
{
    public class ProdutosPaginadosViewModel
    {
        public List<ProdutoViewModel> Produtos { get; set; } = new();
        public int TotalItens { get; set; }
        public int PaginaAtual { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalItens / TamanhoPagina);
        public bool TemPaginaAnterior => PaginaAtual > 1;
        public bool TemProximaPagina => PaginaAtual < TotalPaginas;
    }
}
