using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly IProdutoServices _produtoServices;
        private readonly ICorrelationLogger _correlationLogger;

        public ProdutosController(IProdutoServices produtoServices, ICorrelationLogger correlationLogger)
        {
            _produtoServices = produtoServices;
            _correlationLogger = correlationLogger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            _correlationLogger.LogInformation("Acessando listagem de produtos. Página: {Page}, Tamanho: {PageSize}", page, pageSize);

            try
            {
                var pageWrapper = new PageWrapper
                {
                    Page = page,
                    PageSize = pageSize,
                    Status = "Ativo"
                };

                var produtos = await _produtoServices.ObterProdutosPaginadosAsync(pageWrapper);
                
                return View(produtos);
            }
            catch (Exception ex)
            {
                _correlationLogger.LogError(ex, "Erro ao carregar listagem de produtos");
                TempData["ErrorMessage"] = "Erro ao carregar produtos. Tente novamente.";
                return View(new ProdutosPaginadosViewModel());
            }
        }

        [HttpGet]
        public IActionResult Adicionar()
        {
            _correlationLogger.LogInformation("Acessando formulário de adição de produto");
            return View(new ProdutoModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(ProdutoModel model)
        {
            _correlationLogger.LogInformation("Tentativa de adição de novo produto: {Nome}", model.Nome);

            if (!ModelState.IsValid)
            {
                _correlationLogger.LogWarning("Dados inválidos para adição de produto: {Nome}", model.Nome);
                return View(model);
            }

            try
            {
                var sucesso = await _produtoServices.AdicionarProdutoAsync(model);

                if (sucesso)
                {
                    _correlationLogger.LogInformation("Produto adicionado com sucesso: {Nome}", model.Nome);
                    TempData["SuccessMessage"] = "Produto adicionado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _correlationLogger.LogWarning("Falha ao adicionar produto: {Nome}", model.Nome);
                    TempData["ErrorMessage"] = "Erro ao adicionar produto. Tente novamente.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _correlationLogger.LogError(ex, "Erro ao adicionar produto: {Nome}", model.Nome);
                TempData["ErrorMessage"] = "Erro ao adicionar produto. Tente novamente.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(Guid id)
        {
            _correlationLogger.LogInformation("Acessando detalhes do produto: {Id}", id);

            try
            {
                var produto = await _produtoServices.ObterProdutoPorIdAsync(id);

                if (produto == null)
                {
                    _correlationLogger.LogWarning("Produto não encontrado: {Id}", id);
                    TempData["ErrorMessage"] = "Produto não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(produto);
            }
            catch (Exception ex)
            {
                _correlationLogger.LogError(ex, "Erro ao carregar detalhes do produto: {Id}", id);
                TempData["ErrorMessage"] = "Erro ao carregar detalhes do produto.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
