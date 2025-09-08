namespace Web.Models
{
    public class PageWrapper
    {
        public int Page { get; set; } = 1;
        public int Skip { get; set; }
        public int PageSize { get; set; } = 10;
        public string Status { get; set; } = "Ativo";
    }
}
