using System.Text.Json.Serialization;

namespace OrderService.Models
{
    public class OrderPageRequest
    {
        [JsonPropertyName("page")]
        public int Page { get; set; } = 1;
        [JsonPropertyName("skip")]
        public int Skip { get; set; } = 0;
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 10;
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    public class OrderPageResponse<T>
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; }
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
        [JsonPropertyName("dados")]
        public List<T> Dados { get; set; } = new List<T>();
    }
}