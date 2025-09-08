using Microsoft.AspNetCore.Mvc;
using Web.Filter;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    [ServiceFilter(typeof(ValidarTokenFilter))]
    public class OrdensController(IProdutoServices produtoServices, IOrderService orderService, ICorrelationLogger correlationLogger) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var wrapper = new PageWrapper { Page = 1, PageSize = 100, Status = "Ativo" };
            var produtosPaginados = await produtoServices.ObterProdutosPaginadosAsync(wrapper);
            
            var viewModel = new OrdemServicoViewModel();
            
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProdutos(string termo)
        {
            try
            {
                var wrapper = new PageWrapper { Page = 1, PageSize = 50, Status = "Ativo" };
                var produtosPaginados = await produtoServices.ObterProdutosPaginadosAsync(wrapper);
                
                var produtosFiltrados = produtosPaginados.Produtos
                    .Where(p => p.Nome.ToLower().Contains(termo.ToLower()))
                    .Select(p => new
                    {
                        id = p.Id,
                        nome = p.Nome,
                        valor = p.Valor,
                        quantidadeEstoque = p.QuantidadeEstoque
                    })
                    .ToList();

                return Json(produtosFiltrados);
            }
            catch (Exception ex)
            {
                correlationLogger.LogError(ex, "Erro ao buscar produtos para ordem");
                return Json(new List<object>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> GerarOrdem([FromBody] OrdemServicoViewModel model)
        {
            if (!ModelState.IsValid || model.ProdutosSelecionados == null || !model.ProdutosSelecionados.Any())
            {
                return Json(new { success = false, message = "Lista de produtos inválida" });
            }

            try
            {
                var produtosParaOrdem = model.ProdutosSelecionados.Select(p => new ProdutoParaOrdem
                {
                    ProdutoId = p.ProdutoId,
                    Quantidade = p.Quantidade
                }).ToList();

                var result = await orderService.GerarOrdemServico(produtosParaOrdem);

                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Ordem de serviço gerada com sucesso!", orderId = result.Dados });
                }
                else
                {
                    return Json(new { success = false, message = result.Errors?.FirstOrDefault()?.Description ?? "Erro desconhecido" });
                }
            }
            catch (Exception ex)
            {
                correlationLogger.LogError(ex, "Erro ao gerar ordem de serviço");
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }
    }
}
