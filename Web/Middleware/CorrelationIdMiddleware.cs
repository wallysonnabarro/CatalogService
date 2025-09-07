using System.Diagnostics;

namespace Web.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeaderName = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Verifica se já existe um Correlation ID no header da requisição
            var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault();

            // Se não existir, gera um novo
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            // Adiciona o Correlation ID ao contexto da requisição
            context.Items["CorrelationId"] = correlationId;

            // Adiciona o Correlation ID ao header de resposta
            context.Response.Headers[CorrelationIdHeaderName] = correlationId;

            // Adiciona o Correlation ID ao Activity (para tracing distribuído)
            Activity.Current?.SetTag("correlation.id", correlationId);

            await _next(context);
        }
    }

    // Extensão para facilitar o uso do middleware
    public static class CorrelationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}

