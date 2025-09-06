namespace Web.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
        public string? CorrelationId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public bool ShowErrorMessage => !string.IsNullOrEmpty(ErrorMessage);
        public bool ShowErrorCode => !string.IsNullOrEmpty(ErrorCode);
        public bool ShowCorrelationId => !string.IsNullOrEmpty(CorrelationId);
    }
}
