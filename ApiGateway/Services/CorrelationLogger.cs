using System.Diagnostics;

namespace ApiGateway.Services
{
    public interface ICorrelationLogger
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogError(Exception exception, string message, params object[] args);
    }

    public class CorrelationLogger : ICorrelationLogger
    {
        private readonly ILogger<CorrelationLogger> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationLogger(ILogger<CorrelationLogger> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCorrelationId()
        {
            var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString();
            return string.IsNullOrEmpty(correlationId) ? "N/A" : correlationId;
        }

        public void LogInformation(string message, params object[] args)
        {
            var correlationId = GetCorrelationId();
            _logger.LogInformation($"[CorrelationId: {correlationId}] {message}", args);
        }

        public void LogWarning(string message, params object[] args)
        {
            var correlationId = GetCorrelationId();
            _logger.LogWarning($"[CorrelationId: {correlationId}] {message}", args);
        }

        public void LogError(string message, params object[] args)
        {
            var correlationId = GetCorrelationId();
            _logger.LogError($"[CorrelationId: {correlationId}] {message}", args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            var correlationId = GetCorrelationId();
            _logger.LogError(exception, $"[CorrelationId: {correlationId}] {message}", args);
        }
    }
}
