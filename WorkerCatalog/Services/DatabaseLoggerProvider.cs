using WorkerCatalog.Data;
using WorkerCatalog.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkerCatalog.Services
{
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider _serviceProvider;
        
        public DatabaseLoggerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(categoryName, _serviceProvider);
        }
        
        public void Dispose() { }
    }

    public class DatabaseLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IServiceProvider _serviceProvider;
        
        public DatabaseLogger(string categoryName, IServiceProvider serviceProvider)
        {
            _categoryName = categoryName;
            _serviceProvider = serviceProvider;
        }
        
        public IDisposable BeginScope<TState>(TState state) => null;
        
        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, 
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            
            var message = formatter(state, exception);
            var correlationId = GetCorrelationId();
            
            // Salvar no banco de forma assíncrona
            Task.Run(async () => await SaveLogAsync(logLevel, message, exception, correlationId));
        }
        
        private async Task SaveLogAsync(LogLevel logLevel, string message, Exception exception, string correlationId)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<LogContextDb>();
                
                var log = new ApplicationLog
                {
                    CorrelationId = correlationId,
                    ServiceName = "WorkerCatalog",
                    LogLevel = logLevel.ToString(),
                    Message = message,
                    Exception = exception?.ToString(),
                    Timestamp = DateTime.UtcNow
                };
                
                context.ApplicationLogs.Add(log);
                await context.SaveChangesAsync();
            }
            catch
            {
                // Fallback para console se falhar
                Console.WriteLine($"[{logLevel}] {message}");
            }
        }
        
        private string GetCorrelationId()
        {
            try
            {
                // Para Worker, gerar um Correlation ID único por execução
                return $"WORKER-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString()[..8]}";
            }
            catch
            {
                return "N/A";
            }
        }
    }
}
