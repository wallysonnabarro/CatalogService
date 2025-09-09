using OrderService.Models;

namespace OrderService.Services
{
    public class OrderMappingService : IOrderMappingService
    {
        public OrdemServicoListResponse MapToListResponse(OrdermServico ordem)
        {
            return new OrdemServicoListResponse
            {
                Id = ordem.Id,
                DataHoraRegistro = ordem.DataHoraRegistro,
                Status = ordem.Status,
                Total = ordem.Total,
                QuantidadeProdutos = ordem.Produtos?.Count ?? 0
            };
        }

        public OrdemDetalhesResponse MapToDetailsResponse(OrdermServico ordem)
        {
            return new OrdemDetalhesResponse
            {
                Id = ordem.Id,
                DataHoraRegistro = ordem.DataHoraRegistro,
                Status = ordem.Status,
                Total = ordem.Total,
                Produtos = ordem.Produtos?.Select(p => new ProdutoOrdemResponse
                {
                    ProdutoId = p.ProdutoId,
                    NomeProduto = $"Produto {p.ProdutoId}", // Será preenchido com dados do CatalogService
                    Quantidade = p.Quantidade,
                    Valor = p.Valor
                }).ToList() ?? new List<ProdutoOrdemResponse>()
            };
        }

        public List<OrdemServicoListResponse> MapToListResponse(List<OrdermServico> ordens)
        {
            return ordens.Select(MapToListResponse).ToList();
        }
    }
}