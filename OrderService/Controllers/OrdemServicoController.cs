using Microsoft.AspNetCore.Mvc;
using OrderService.Aplicacao;
using OrderService.Models;

namespace OrderService.Controllers
{
    [Route("orders/orderns")]
    [ApiController]
    public class OrdemServicoController(IOrdemServicoUseCase _useCase) : ControllerBase
    {
        [HttpPost]
        [Route("gerar-ordem")]
        public async Task<IActionResult> GerarOrdemServico(List<Produto> produtos)
        {
            var listaGuids = await _useCase.Executar(produtos);

            return Ok(listaGuids);
        }
    }
}
