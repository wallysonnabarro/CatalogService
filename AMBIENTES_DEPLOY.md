# 🚀 Guia de Configuração de Ambientes

Este documento explica como configurar e executar o projeto em diferentes ambientes (Desenvolvimento e Produção).

## 📁 Estrutura de Configuração

### Arquivos de Configuração
```
├── docker-compose.yml          # Ambiente de DESENVOLVIMENTO
├── docker-compose.prod.yml     # Ambiente de PRODUÇÃO
├── start-dev.ps1              # Script para desenvolvimento
├── start-prod.ps1             # Script para produção
└── AMBIENTES_DEPLOY.md        # Este arquivo
```

### AppSettings por Projeto
Cada projeto possui 3 arquivos de configuração:
- `appsettings.json` - Configurações base
- `appsettings.Development.json` - Configurações de desenvolvimento
- `appsettings.Production.json` - Configurações de produção

## 🛠️ Como Usar

### 1. Ambiente de Desenvolvimento (Padrão)

**Para desenvolvimento local e testes:**

```powershell
# Opção 1: Usar o script (Recomendado)
.\start-dev.ps1

# Opção 2: Comando direto
docker-compose up --build -d
```

**Características:**
- ✅ Utiliza `appsettings.Development.json`
- ✅ Logs detalhados
- ✅ Swagger habilitado
- ✅ Debugging ativo
- ✅ Configurações de desenvolvimento

### 2. Ambiente de Produção

**Para deploy em produção:**

```powershell
# Opção 1: Usar o script (Recomendado)
.\start-prod.ps1

# Opção 2: Comando direto
docker-compose -f docker-compose.prod.yml up --build -d
```

**Características:**
- ✅ Utiliza `appsettings.Production.json`
- ✅ Logs otimizados
- ✅ Swagger desabilitado
- ✅ Configurações de produção
- ✅ Performance otimizada

## 🔧 Diferenças Entre Ambientes

### Desenvolvimento (`docker-compose.yml`)
```yaml
environment:
  ASPNETCORE_ENVIRONMENT: "Development"
```

### Produção (`docker-compose.prod.yml`)
```yaml
environment:
  ASPNETCORE_ENVIRONMENT: "Production"
```

## 📋 Comandos Úteis

### Parar Ambientes
```powershell
# Parar desenvolvimento
docker-compose down

# Parar produção
docker-compose -f docker-compose.prod.yml down
```

### Ver Logs
```powershell
# Ver logs de desenvolvimento
docker-compose logs -f

# Ver logs de produção
docker-compose -f docker-compose.prod.yml logs -f
```

### Limpar Sistema
```powershell
# Limpar containers, redes e volumes
docker system prune -a

# Limpar apenas containers parados
docker container prune
```

## 🌐 URLs de Acesso

### Desenvolvimento e Produção
- **Web Application**: http://localhost:5002
- **API Gateway**: http://localhost:5000
- **OAuth Services**: http://localhost:5004
- **RabbitMQ Management**: http://localhost:15672

### Bancos de Dados
- **SQL1 (OrderService)**: localhost:14333
- **SQL2 (CatalogService)**: localhost:14334
- **SQL3 (ApiGateway)**: localhost:14335
- **SQL4 (OAuthServices)**: localhost:14336
- **SQL5 (Web Logs)**: localhost:14337

## 🔐 Configurações de Produção

### Variáveis de Ambiente Importantes
- `ASPNETCORE_ENVIRONMENT=Production`
- `TZ=America/Sao_Paulo`
- `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false`

### Configurações de Logging
- **Desenvolvimento**: Logs detalhados, Debug ativo
- **Produção**: Logs otimizados, apenas Information e acima

## 🚨 Troubleshooting

### Problema: Container não inicia
```powershell
# Verificar logs
docker-compose logs [nome-do-serviço]

# Reconstruir sem cache
docker-compose build --no-cache
```

### Problema: Porta já em uso
```powershell
# Verificar processos usando a porta
netstat -ano | findstr :5000

# Parar processo específico
taskkill /PID [PID] /F
```

### Problema: Banco de dados não conecta
```powershell
# Verificar se SQL Server está rodando
docker ps | findstr sql

# Verificar logs do SQL Server
docker logs sql1
```

## 📝 Notas Importantes

1. **Sempre use os scripts** `start-dev.ps1` e `start-prod.ps1` para facilitar o deploy
2. **Ambiente de desenvolvimento** é o padrão quando você executa `docker-compose up`
3. **Ambiente de produção** requer o arquivo `docker-compose.prod.yml`
4. **Configurações de produção** são mais restritivas e otimizadas
5. **Logs de produção** são menos verbosos para melhor performance

## 🎯 Resumo dos Comandos

| Ação | Desenvolvimento | Produção |
|------|----------------|----------|
| **Iniciar** | `.\start-dev.ps1` | `.\start-prod.ps1` |
| **Parar** | `docker-compose down` | `docker-compose -f docker-compose.prod.yml down` |
| **Logs** | `docker-compose logs -f` | `docker-compose -f docker-compose.prod.yml logs -f` |
| **Rebuild** | `docker-compose up --build -d` | `docker-compose -f docker-compose.prod.yml up --build -d` |

---

**Desenvolvedor**: Wallyson Lopes - Sênior

