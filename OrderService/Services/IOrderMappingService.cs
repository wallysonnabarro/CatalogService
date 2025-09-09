using OrderService.Models;

namespace OrderService.Services
{
    public interface IOrderMappingService
    {
        OrdemServicoListResponse MapToListResponse(OrdermServico ordem);
        OrdemDetalhesResponse MapToDetailsResponse(OrdermServico ordem);
        List<OrdemServicoListResponse> MapToListResponse(List<OrdermServico> ordens);
    }
}