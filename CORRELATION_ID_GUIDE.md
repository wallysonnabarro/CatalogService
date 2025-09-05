# Guia de Implementação - Correlation ID / Trace ID

## Visão Geral

Este documento descreve a implementação do sistema de Correlation ID / Trace ID no sistema de microserviços. O Correlation ID permite rastrear requisições através de múltiplos serviços, facilitando o debugging e monitoramento.

## Arquitetura Implementada

### Fluxo de Requisição

```
Cliente → ApiGateway → OAuthServices (se necessário)
                ↓
            CatalogService / OrderService
```

### Componentes Implementados

1. **Middleware de Correlation ID** - Em todos os serviços
2. **Serviço de Logging** - Com Correlation ID integrado
3. **Propagação de Headers** - No ApiGateway
4. **Logging Estruturado** - Em todos os controllers

## Como Funciona

### 1. Geração do Correlation ID

- **Primeira requisição**: Se não houver `X-Correlation-ID` no header, um novo GUID é gerado
- **Requisições subsequentes**: O Correlation ID é propagado através de todos os serviços

### 2. Propagação

- O ApiGateway propaga o Correlation ID para os serviços downstream
- Cada serviço adiciona o Correlation ID aos headers de resposta
- O Correlation ID é mantido durante toda a jornada da requisição

### 3. Logging

- Todos os logs incluem o Correlation ID no formato: `[CorrelationId: {id}]`
- Facilita a busca e correlação de logs entre serviços

## Estrutura de Arquivos

```
├── ApiGateway/
│   ├── Middleware/CorrelationIdMiddleware.cs
│   ├── Services/CorrelationLogger.cs
│   └── Program.cs (atualizado)
├── OAuthServices/
│   ├── Middleware/CorrelationIdMiddleware.cs
│   ├── Services/CorrelationLogger.cs
│   ├── Controllers/AuthController.cs (atualizado)
│   └── Program.cs (atualizado)
├── CatalogService/
│   ├── Middleware/CorrelationIdMiddleware.cs
│   ├── Services/CorrelationLogger.cs
│   ├── Controllers/ProdutosController.cs (atualizado)
│   └── Program.cs (atualizado)
└── OrderService/
    ├── Middleware/CorrelationIdMiddleware.cs
    ├── Services/CorrelationLogger.cs
    ├── Controllers/OrdemServicoController.cs (atualizado)
    └── Program.cs (atualizado)
```

## Como Usar

### 1. Em Controllers

```csharp
public class MeuController(ICorrelationLogger _logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("Iniciando processamento da requisição");
        
        try
        {
            // Sua lógica aqui
            _logger.LogInformation("Processamento concluído com sucesso");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o processamento");
            throw;
        }
    }
}
```

### 2. Em Serviços

```csharp
public class MeuServico(ICorrelationLogger _logger)
{
    public async Task Processar()
    {
        _logger.LogInformation("Iniciando processamento no serviço");
        // Sua lógica aqui
    }
}
```

### 3. Headers HTTP

O sistema automaticamente:
- Adiciona `X-Correlation-ID` aos headers de resposta
- Propaga o header para serviços downstream
- Mantém o mesmo ID durante toda a jornada da requisição

## Exemplo de Logs

```
[CorrelationId: 123e4567-e89b-12d3-a456-426614174000] Iniciando processo de autenticação para client_id: meu-client
[CorrelationId: 123e4567-e89b-12d3-a456-426614174000] Credenciais validadas com sucesso para client_id: meu-client
[CorrelationId: 123e4567-e89b-12d3-a456-426614174000] Token JWT gerado com sucesso para client_id: meu-client
[CorrelationId: 123e4567-e89b-12d3-a456-426614174000] Buscando produto por ID: 456e7890-e89b-12d3-a456-426614174001
[CorrelationId: 123e4567-e89b-12d3-a456-426614174000] Produto encontrado com sucesso para ID: 456e7890-e89b-12d3-a456-426614174001
```

## Benefícios

1. **Rastreabilidade**: Rastreie uma requisição através de todos os serviços
2. **Debugging**: Facilite a identificação de problemas em requisições específicas
3. **Monitoramento**: Correlacione métricas e logs entre serviços
4. **Auditoria**: Mantenha um histórico completo de cada requisição

## Configuração Adicional

### Para Adicionar em Novos Serviços

1. Copie o `CorrelationIdMiddleware.cs` para o novo serviço
2. Copie o `CorrelationLogger.cs` para o novo serviço
3. Adicione as configurações no `Program.cs`:
   ```csharp
   builder.Services.AddHttpContextAccessor();
   builder.Services.AddScoped<ICorrelationLogger, CorrelationLogger>();
   app.UseCorrelationId();
   ```

### Para Integração com Sistemas de Monitoramento

O Correlation ID também é adicionado ao `Activity.Current` para integração com:
- OpenTelemetry
- Application Insights
- Jaeger
- Zipkin

## Testando

Para testar o sistema:

1. Faça uma requisição para qualquer endpoint
2. Verifique se o header `X-Correlation-ID` está presente na resposta
3. Verifique os logs para confirmar que o mesmo ID aparece em todos os serviços
4. Faça requisições subsequentes para verificar a propagação

## Considerações de Performance

- O middleware adiciona overhead mínimo
- O Correlation ID é gerado apenas uma vez por requisição
- O logging estruturado é otimizado para performance
