using System.Text;
using System.Text.Json;
using Web.Models;

namespace Web.Services
{
    public class ProdutoServices(IHttpClientFactory httpClientFactory, ICorrelationLogger correlationLogger) : IProdutoServices
    {
        public async Task<ProdutosPaginadosViewModel> ObterProdutosPaginadosAsync(PageWrapper pageWrapper)
        {
            correlationLogger.LogInformation("Iniciando busca de produtos paginados. Página: {Page}, Tamanho: {PageSize}", 
                pageWrapper.Page, pageWrapper.PageSize);

            try
            {
                using var client = httpClientFactory.CreateClient("gatewayservices");

                var jsonContent = JsonSerializer.Serialize(pageWrapper);
                var url = "/catalog/produtos/produtos-paginado";

                correlationLogger.LogInformation("Enviando requisição para buscar produtos paginados. URL: {Url}", url);

                var responseMessage = await client.PostAsync(url,
                    new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    var paginacaoResponse = JsonSerializer.Deserialize<PaginacaoResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var produtos = paginacaoResponse?.Dados ?? new List<ProdutoViewModel>();
                    
                    var viewModel = new ProdutosPaginadosViewModel
                    {
                        Produtos = produtos,
                        PaginaAtual = pageWrapper.Page,
                        TamanhoPagina = pageWrapper.PageSize,
                        TotalItens = paginacaoResponse?.Count ?? 0
                    };

                    correlationLogger.LogInformation("Produtos paginados obtidos com sucesso. Total: {Total}", viewModel.TotalItens);
                    return viewModel;
                }
                else
                {
                    correlationLogger.LogWarning("Falha ao obter produtos paginados. Status: {StatusCode}, Response: {Response}", 
                        responseMessage.StatusCode, responseContent);
                    
                    return new ProdutosPaginadosViewModel
                    {
                        Produtos = new List<ProdutoViewModel>(),
                        PaginaAtual = pageWrapper.Page,
                        TamanhoPagina = pageWrapper.PageSize,
                        TotalItens = 0
                    };
                }
            }
            catch (Exception e)
            {
                correlationLogger.LogError(e, "Erro ao obter produtos paginados");
                throw new Exception("Erro ao buscar produtos: " + e.Message);
            }
        }

        public async Task<bool> AdicionarProdutoAsync(ProdutoModel produto)
        {
            correlationLogger.LogInformation("Iniciando adição de novo produto: {Nome}", produto.Nome);

            try
            {
                using var client = httpClientFactory.CreateClient("gatewayservices");

                var jsonContent = JsonSerializer.Serialize(produto);
                var url = "/catalog/produtos/adicionar-produto";

                correlationLogger.LogInformation("Enviando requisição para adicionar produto. URL: {Url}, Nome: {Nome}", url, produto.Nome);

                var responseMessage = await client.PostAsync(url,
                    new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    correlationLogger.LogInformation("Produto adicionado com sucesso: {Nome}", produto.Nome);
                    return true;
                }
                else
                {
                    correlationLogger.LogWarning("Falha ao adicionar produto: {Nome}. Status: {StatusCode}, Response: {Response}", 
                        produto.Nome, responseMessage.StatusCode, responseContent);
                    return false;
                }
            }
            catch (Exception e)
            {
                correlationLogger.LogError(e, "Erro ao adicionar produto: {Nome}", produto.Nome);
                throw new Exception("Erro ao adicionar produto: " + e.Message);
            }
        }

        public async Task<ProdutoViewModel?> ObterProdutoPorIdAsync(Guid id)
        {
            correlationLogger.LogInformation("Iniciando busca de produto por ID: {Id}", id);

            try
            {
                using var client = httpClientFactory.CreateClient("gatewayservices");

                var url = $"/catalog/produtos/por-id/{id}";

                correlationLogger.LogInformation("Enviando requisição para buscar produto por ID. URL: {Url}", url);

                var responseMessage = await client.GetAsync(url);
                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    var produto = JsonSerializer.Deserialize<ProdutoViewModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    correlationLogger.LogInformation("Produto encontrado com sucesso para ID: {Id}", id);
                    return produto;
                }
                else
                {
                    correlationLogger.LogWarning("Produto não encontrado para ID: {Id}. Status: {StatusCode}, Response: {Response}", 
                        id, responseMessage.StatusCode, responseContent);
                    return null;
                }
            }
            catch (Exception e)
            {
                correlationLogger.LogError(e, "Erro ao buscar produto por ID: {Id}", id);
                throw new Exception("Erro ao buscar produto: " + e.Message);
            }
        }
    }
}
