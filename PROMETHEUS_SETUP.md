# Guia de Implementação do Prometheus para Monitoramento de Microserviços

## Visão Geral

Este documento fornece instruções detalhadas para implementar o Prometheus na infraestrutura de containers para monitoramento dos microserviços .NET.

## 1. Configuração do Prometheus Server

### Passo 1: Criar arquivo de configuração do Prometheus

Crie um arquivo `prometheus.yml` na raiz do seu projeto:

```yaml
global:
  scrape_interval: 15s
  evaluation_interval: 15s

rule_files:
  # - "first_rules.yml"
  # - "second_rules.yml"

scrape_configs:
  - job_name: 'prometheus'
    static_configs:
      - targets: ['localhost:9090']

  - job_name: 'catalog-service'
    static_configs:
      - targets: ['catalogservice:80']
    metrics_path: '/metrics'
    scrape_interval: 10s

  - job_name: 'order-service'
    static_configs:
      - targets: ['orderservice:80']
    metrics_path: '/metrics'
    scrape_interval: 10s

  - job_name: 'oauth-service'
    static_configs:
      - targets: ['oauthservices:80']
    metrics_path: '/metrics'
    scrape_interval: 10s

  - job_name: 'api-gateway'
    static_configs:
      - targets: ['apigateway:80']
    metrics_path: '/metrics'
    scrape_interval: 10s
```

## 2. Configuração do Grafana

### Passo 2: Criar arquivo de configuração do Grafana

Crie um arquivo `grafana-datasources.yml`:

```yaml
apiVersion: 1

datasources:
  - name: Prometheus
    type: prometheus
    access: proxy
    url: http://prometheus:9090
    isDefault: true
    editable: true
```

### Passo 3: Dashboard básico do Grafana

Crie um arquivo `grafana-dashboard.json` com um dashboard básico para monitorar seus microserviços.

## 3. Instrumentação dos Microserviços .NET

### Passo 4: Adicionar pacotes NuGet

Para cada projeto de microserviço, adicione os seguintes pacotes:

```xml
<PackageReference Include="prometheus-net" Version="8.2.1" />
<PackageReference Include="prometheus-net.AspNetCore" Version="8.2.1" />
<PackageReference Include="prometheus-net.SystemMetrics" Version="8.2.1" />
```

### Passo 5: Configurar no Program.cs

Para cada microserviço, adicione no `Program.cs`:

```csharp
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Adicionar outros serviços existentes
builder.Services.AddControllers();
// ... seus outros serviços

var app = builder.Build();

// Configurar middleware do Prometheus
app.UseHttpMetrics();
app.UseRouting();
app.MapMetrics(); // Endpoint /metrics

// Seus middlewares existentes
app.UseCorrelationIdMiddleware();

app.MapControllers();

app.Run();
```

### Configuração Alternativa (se necessário)

Se você quiser mais controle sobre as métricas, pode usar esta configuração:

```csharp
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Adicionar outros serviços existentes
builder.Services.AddControllers();
// ... seus outros serviços

var app = builder.Build();

// Configurar métricas HTTP
app.UseHttpMetrics(options =>
{
    options.AddCustomLabel(new HttpCustomLabel("service", () => Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "catalog-service")); // Nome do seu serviço
});

app.UseRouting();
app.MapMetrics(); // Endpoint /metrics

// Seus middlewares existentes
app.UseCorrelationIdMiddleware();

app.MapControllers();

app.Run();
```

### Passo 6: Métricas customizadas

Exemplo de métricas customizadas no controller:

```csharp
using Prometheus;

public class ProdutosController : ControllerBase
{
    private static readonly Counter RequestCount = Metrics
        .CreateCounter("produtos_requests_total", "Total de requisições para produtos");
    
    private static readonly Histogram RequestDuration = Metrics
        .CreateHistogram("produtos_request_duration_seconds", "Duração das requisições de produtos");

    [HttpGet]
    public async Task<IActionResult> GetProdutos()
    {
        using (RequestDuration.NewTimer())
        {
            RequestCount.Inc();
            
            // Sua lógica existente
            var produtos = await _produtosRepository.GetAllAsync();
            return Ok(produtos);
        }
    }
}
```

## 4. Configuração do Docker Compose

### Passo 7: Adicionar ao docker-compose.yml

Adicione os seguintes serviços ao seu `docker-compose.yml`:

```yaml
version: '3.8'

services:
  # Seus serviços existentes...
  
  prometheus:
    image: prom/prometheus:latest
    container_name: prometheus
    ports:
      - "9090:9090"
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml
      - prometheus_data:/prometheus
    command:
      - '--config.file=/etc/prometheus/prometheus.yml'
      - '--storage.tsdb.path=/prometheus'
      - '--web.console.libraries=/etc/prometheus/console_libraries'
      - '--web.console.templates=/etc/prometheus/consoles'
      - '--storage.tsdb.retention.time=200h'
      - '--web.enable-lifecycle'
    networks:
      - microservices-network

  grafana:
    image: grafana/grafana:latest
    container_name: grafana
    ports:
      - "3000:3000"
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin123
    volumes:
      - grafana_data:/var/lib/grafana
      - ./grafana-datasources.yml:/etc/grafana/provisioning/datasources/datasources.yml
    networks:
      - microservices-network
    depends_on:
      - prometheus

  node-exporter:
    image: prom/node-exporter:latest
    container_name: node-exporter
    ports:
      - "9100:9100"
    volumes:
      - /proc:/host/proc:ro
      - /sys:/host/sys:ro
      - /:/rootfs:ro
    command:
      - '--path.procfs=/host/proc'
      - '--path.rootfs=/rootfs'
      - '--path.sysfs=/host/sys'
      - '--collector.filesystem.mount-points-exclude=^/(sys|proc|dev|host|etc)($$|/)'
    networks:
      - microservices-network

volumes:
  prometheus_data:
  grafana_data:

networks:
  microservices-network:
    driver: bridge
```

### Passo 8: Atualizar prometheus.yml

Atualize o `prometheus.yml` para incluir o node-exporter:

```yaml
scrape_configs:
  # Configurações existentes...
  
  - job_name: 'node-exporter'
    static_configs:
      - targets: ['node-exporter:9100']
```

## 5. Métricas Essenciais para Monitorar

### Métricas de Aplicação:
- **HTTP Requests**: `http_requests_total`, `http_request_duration_seconds`
- **Business Metrics**: `produtos_criados_total`, `pedidos_processados_total`
- **Error Rates**: `http_requests_errors_total`
- **Database Connections**: `database_connections_active`

### Métricas de Sistema:
- **CPU Usage**: `node_cpu_seconds_total`
- **Memory Usage**: `node_memory_MemAvailable_bytes`
- **Disk Usage**: `node_filesystem_avail_bytes`
- **Network I/O**: `node_network_receive_bytes_total`

### Alertas Recomendados:

Criar arquivo `alerts.yml`:

```yaml
groups:
- name: microservices
  rules:
  - alert: HighErrorRate
    expr: rate(http_requests_errors_total[5m]) > 0.1
    for: 5m
    labels:
      severity: warning
    annotations:
      summary: "Alta taxa de erro no serviço"
      
  - alert: HighResponseTime
    expr: histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m])) > 1
    for: 5m
    labels:
      severity: warning
    annotations:
      summary: "Tempo de resposta alto"
```

## 6. Comandos para Executar

### Passo 9: Comandos de inicialização

```powershell
# Subir a infraestrutura de monitoramento
docker-compose up -d prometheus grafana node-exporter

# Verificar se os serviços estão rodando
docker-compose ps

# Ver logs do Prometheus
docker-compose logs prometheus

# Ver logs do Grafana
docker-compose logs grafana
```

### Passo 10: Acessar as interfaces

- **Prometheus**: http://localhost:9090
- **Grafana**: http://localhost:3000 (admin/admin123)
- **Node Exporter**: http://localhost:9100/metrics

### Passo 11: Verificar métricas dos microserviços

Acesse os endpoints de métricas de cada serviço:
- http://localhost:5001/metrics (CatalogService)
- http://localhost:5002/metrics (OrderService)
- http://localhost:5003/metrics (OAuthService)
- http://localhost:5004/metrics (ApiGateway)

## 7. Próximos Passos

1. **Configure dashboards no Grafana** para visualizar as métricas
2. **Implemente alertas** para notificações automáticas
3. **Adicione métricas de negócio** específicas da sua aplicação
4. **Configure retenção de dados** adequada para seu ambiente
5. **Implemente backup** das configurações do Grafana

## 8. Estrutura de Arquivos

Após a implementação, sua estrutura de arquivos deve incluir:

```
CatalogService/
├── prometheus.yml
├── grafana-datasources.yml
├── alerts.yml
├── docker-compose.yml
├── CatalogService/
│   ├── Program.cs (atualizado)
│   └── Controllers/ (com métricas)
├── OrderService/
│   ├── Program.cs (atualizado)
│   └── Controllers/ (com métricas)
├── OAuthServices/
│   ├── Program.cs (atualizado)
│   └── Controllers/ (com métricas)
└── ApiGateway/
    ├── Program.cs (atualizado)
    └── Controllers/ (com métricas)
```

## 9. Troubleshooting

### Problemas Comuns:

1. **Erro: "AddPrometheusCounters", "AddPrometheusHistograms" ou "AddRouteParameters" não existe**
   - **Solução**: Esses métodos/propriedades não existem na biblioteca `prometheus-net`
   - Use apenas `app.UseHttpMetrics()` e `app.MapMetrics()`
   - As métricas são criadas automaticamente
   - Para labels customizados, use `new HttpCustomLabel("nome", () => "valor")` dentro de `AddCustomLabel()`

2. **Erro com AddCustomLabel**
   - **Solução**: Use `new HttpCustomLabel("nome", () => "valor")` como parâmetro
   - O delegate não recebe argumentos: `() => "valor"` (não `(arg) => "valor"`)
   - Exemplo: `options.AddCustomLabel(new HttpCustomLabel("service", () => Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "catalog-service"));`

3. **Métricas não aparecem no Prometheus**
   - Verifique se o endpoint `/metrics` está acessível
   - Confirme se os serviços estão na mesma rede Docker
   - Teste: `curl http://localhost:5001/metrics`

4. **Grafana não consegue conectar ao Prometheus**
   - Verifique se o Prometheus está rodando
   - Confirme a URL no datasource

5. **Node Exporter não coleta métricas**
   - Verifique as permissões de volume
   - Confirme se o container tem acesso aos diretórios do sistema

6. **Erro de compilação com prometheus-net**
   - Verifique se adicionou o `using Prometheus;`
   - Confirme se os pacotes NuGet foram instalados corretamente
   - Versão recomendada: `prometheus-net.AspNetCore` 8.2.1

## 10. Recursos Adicionais

- [Documentação oficial do Prometheus](https://prometheus.io/docs/)
- [Documentação do Grafana](https://grafana.com/docs/)
- [prometheus-net para .NET](https://github.com/prometheus-net/prometheus-net)
- [Exemplos de dashboards do Grafana](https://grafana.com/grafana/dashboards/)

---

**Nota**: Este guia fornece uma base sólida para monitoramento. Ajuste as configurações conforme suas necessidades específicas de ambiente e requisitos de negócio.
