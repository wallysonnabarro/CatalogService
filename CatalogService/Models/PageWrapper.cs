using Swashbuckle.AspNetCore.Annotations;

namespace CatalogService.Models
{
    public class PageWrapper
    {
        [SwaggerSchema("Valor inicial padrão: 1")]
        public int Page { get; set; } = 1;
        public int Skip { get; set; }
        public int PageSize { get; set; }
        [SwaggerSchema("Situação do produto")]
        public SituacaoEnum Status { get; set; } = SituacaoEnum.Ativo;
    }
}
