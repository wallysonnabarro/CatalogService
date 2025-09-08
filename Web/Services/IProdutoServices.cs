using Web.Models;

namespace Web.Services
{
    public interface IProdutoServices
    {
        Task<ProdutosPaginadosViewModel> ObterProdutosPaginadosAsync(PageWrapper pageWrapper);
        Task<bool> AdicionarProdutoAsync(ProdutoModel produto);
        Task<ProdutoViewModel?> ObterProdutoPorIdAsync(Guid id);
    }
}
