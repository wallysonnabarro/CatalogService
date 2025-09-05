using CatalogService.Data;
using CatalogService.Models;
using CatalogService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CatalogService.Controllers
{
    [Route("catalog/produtos")]
    [ApiController]
    public class ProdutosController(IProdutosRepository _produtoRepository, ICorrelationLogger _logger) : ControllerBase
    {
        [HttpGet("por-id/{id}")]
        public async Task<IActionResult> GetProdutosAsync(string id)
        {
            _logger.LogInformation("Buscando produto por ID: {ProdutoId}", id);

            if (String.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Tentativa de busca com ID inválido");
                return BadRequest("Id inválido");
            }

            var produtos = await _produtoRepository.ProdutoPorId(Guid.Parse(id));

            if (produtos.Succeeded)
            {
                _logger.LogInformation("Produto encontrado com sucesso para ID: {ProdutoId}", id);
                return Ok(produtos.Dados);
            }
            else
            {
                _logger.LogWarning("Produto não encontrado para ID: {ProdutoId}. Erro: {Erro}", id, produtos.ToString());
                return BadRequest(produtos.ToString());
            }
        }

        [HttpPost]
        [Route("atualizar-quantidade")]
        public async Task<IActionResult> AtualizarQuantidadeProdutoAsync(ICollection<ProdutoAtualizarQuantidade> lista)
        {
            if (lista == null || !lista.Any())
                return BadRequest("A lista de produtos não pode ser nula ou vazia.");

            try
            {
                await _produtoRepository.AtualizarQuantidadeProdutos(lista);

                return Ok();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Paginar lista de produtos ultilizando PageWrapper. Valores iniciais page: 1, skip: 1, pageSize: 10, status: "Ativo"
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("produtos-paginado")]
        public async Task<IActionResult> ProdutosPaginadoAsync(PageWrapper wrapper)
        {
            if (wrapper == null) return BadRequest("Parâmetros de paginação inválidos.");

            if (wrapper.PageSize == 0)
                wrapper.PageSize = 10;

            if (wrapper.Skip == 0)
                wrapper.Skip = (wrapper.Page - 1) * wrapper.PageSize;

            var produtos = await _produtoRepository.ProdutosPaginado(wrapper);

            if (produtos.Succeeded)
                return Ok(produtos.Dados);
            else return BadRequest(produtos.ToString());
        }

        [HttpPost]
        [Route("atualizar-situacao")]
        public async Task<IActionResult> AtualizarSituacaoProdutoAsync(ProdutoAtualizarSituacao model)
        {
            if (model == null) return BadRequest("Modelo inválido.");

            var result = await _produtoRepository.AtualizarSituacaoProduto(model.Id, Enum.Parse<SituacaoEnum>(model.Status));

            if (!result.Succeeded)
                return BadRequest(result.ToString());

            return Ok();
        }

        /// <summary>
        /// Retorna uma lista de IDs de produtos válidos a partir de uma lista fornecida.
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("validar-lista-produtos")]
        public async Task<IActionResult> ValidarListaProdutosAsync(ICollection<Guid> lista)
        {
            if (lista.Count == 0) return BadRequest("A lista de produtos não pode ser vazia.");

            var produtos = await _produtoRepository.ValidarListaProdutosAsync(lista);
            if (produtos.Succeeded)
                return Ok(produtos.Dados);
            else
                return BadRequest(produtos.ToString());
        }

        [HttpPost]
        [Route("adicionar-produto")]
        public async Task<IActionResult> AdicionarProdutoAsync(ProdutoModel model)
        {
            if (model == null) return BadRequest("Modelo inválido.");

            var produto = await _produtoRepository.AdicionarProduto(model);

            if (!produto.Succeeded)
                return BadRequest(produto.ToString());

            return Ok(JsonSerializer.Serialize(produto.Dados));
        }
    }
}
