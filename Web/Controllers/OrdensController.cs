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
                    return Json(new { success = true, message = "Ordem de serviço gerada com sucesso!", orderId = result.Dados.Id });
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

        [HttpGet]
        public async Task<IActionResult> Listar(int page = 1, int pageSize = 10)
        {
            try
            {
                var pageWrapper = new PageWrapper
                {
                    Page = page,
                    PageSize = pageSize,
                    Skip = (page - 1) * pageSize,
                    Status = "Aberta"
                };

                var ordensPaginadas = await orderService.ListarOrdensPaginadasAsync(pageWrapper);
                return View(ordensPaginadas);
            }
            catch (Exception ex)
            {
                correlationLogger.LogError(ex, "Erro ao buscar lista de ordens");
                TempData["ErrorMessage"] = "Erro ao carregar lista de ordens.";
                return View(new OrdensPaginadasViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(Guid id)
        {
            try
            {
                var ordem = await orderService.ObterOrdemPorIdAsync(id);
                if (ordem == null)
                {
                    TempData["ErrorMessage"] = "Ordem não encontrada.";
                    return RedirectToAction(nameof(Listar));
                }
                return View(ordem);
            }
            catch (Exception ex)
            {
                correlationLogger.LogError(ex, "Erro ao buscar detalhes da ordem {OrderId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar detalhes da ordem.";
                return RedirectToAction(nameof(Listar));
            }
        }
    }
}
