# Configuração Docker e Migrations - Microservices

## Resumo do Projeto

Este documento descreve a configuração completa do ambiente Docker para microservices .NET Core e os procedimentos para gerenciar migrations do Entity Framework Core dentro dos containers.

## 1. Estrutura dos Dockerfiles

### Problema Identificado
Os containers estavam usando a imagem `mcr.microsoft.com/dotnet/aspnet:9.0` (runtime) que não inclui o .NET SDK necessário para executar comandos do Entity Framework Core.

### Solução Implementada
Modificação dos Dockerfiles para usar `mcr.microsoft.com/dotnet/sdk:9.0` na fase final, permitindo:
- Execução da aplicação
- Acesso aos comandos do Entity Framework Core
- Manutenção do código fonte dentro do container

### Exemplo de Dockerfile Modificado

```dockerfile
# Esta fase é usada na produção ou quando executada no VS no modo normal
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final  # Mudança: usar SDK em vez de aspnet
WORKDIR /app

# Copiar arquivos publicados (DLLs)
COPY --from=publish /app/publish .

# Copiar código fonte da fase build para uma pasta separada
COPY --from=build /src/OrderService /src/OrderService

# Instalar dotnet-ef
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "OrderService.dll"]
```

## 2. Docker Compose

### Estrutura dos Serviços
- **sql1**: SQL Server para OrderService
- **sql2**: SQL Server para CatalogService
- **rabbitmq**: Message broker
- **apigateway**: API Gateway
- **catalogservice**: Serviço de catálogo
- **oauthservices**: Serviço de autenticação jwt - por enquanto sem altenticação de usuários, somente chaves
- **orderservice**: Serviço de pedidos
- **workercatalog**: Worker service

### Configuração de Rede
Todos os serviços estão conectados na rede `backend` para comunicação interna.

## 3. Ajustes nos Dockerfiles para Migrations

### Modificações Realizadas
1. **Mudança da imagem base**: De `aspnet` para `sdk`
2. **Cópia do código fonte**: Manutenção do código fonte em `/src/[NomeDoServico]`
3. **Instalação do dotnet-ef**: Ferramenta global para gerenciar migrations
4. **Configuração do PATH**: Adição do diretório de ferramentas ao PATH

### Benefícios
- Acesso completo aos comandos do Entity Framework Core
- Possibilidade de criar e aplicar migrations dentro do container
- Manutenção do código fonte para desenvolvimento
- Compatibilidade com ferramentas de desenvolvimento

## 4. Comandos para Gerenciar Migrations no Container

### Entrada no Container

```bash
# Para OrderService
docker exec -it orderservice bash

# Para CatalogService
docker exec -it catalogservice bash

```

### Verificação do Ambiente

```bash
# Verificar se o dotnet-ef está funcionando
dotnet ef --version
```

### Verificação da Estrutura de Arquivos

```bash
# Verificar se a pasta do código fonte existe
ls -la /src/

# Para OrderService
ls -la /src/OrderService/
ls -la /src/OrderService/*.csproj
ls -la /src/OrderService/Controllers/
ls -la /src/OrderService/Models/
ls -la /src/OrderService/Data/

# Para CatalogService
ls -la /src/CatalogService/
ls -la /src/CatalogService/*.csproj
ls -la /src/CatalogService/Controllers/
ls -la /src/CatalogService/Models/
ls -la /src/CatalogService/Data/
```

### Comandos de Migration

#### Para OrderService
```bash
# Navegar para o diretório do projeto
cd /src/OrderService

# Listar migrations existentes
dotnet ef migrations list

# Aplicar migrations pendentes
dotnet ef database update

# Criar nova migration / Caso não houver nenhuma
dotnet ef migrations add NomeDaMigration

# Remover última migration - se necessário
dotnet ef migrations remove

# Gerar script SQL
dotnet ef migrations script
```

#### Para CatalogService
```bash
# Navegar para o diretório do projeto
cd /src/CatalogService

# Listar migrations existentes
dotnet ef migrations list

# Aplicar migrations pendentes
dotnet ef database update

# Criar nova migration / Caso não houver nenhuma
dotnet ef migrations add NomeDaMigration

# Remover última migration - se necessário
dotnet ef migrations remove

# Gerar script SQL
dotnet ef migrations script
```

#### Para OAuthServices
```bash
# Navegar para o diretório do projeto
cd /src/OAuthServices

# Listar migrations existentes
dotnet ef migrations list

# Aplicar migrations pendentes
dotnet ef database update

# Criar nova migration
dotnet ef migrations add NomeDaMigration
```

## 5. Comandos Úteis do Entity Framework Core

### Comandos Básicos
```bash
# Verificar versão
dotnet ef --version

# Listar migrations
dotnet ef migrations list

# Aplicar migrations
dotnet ef database update

# Criar migration
dotnet ef migrations add NomeDaMigration

# Remover migration
dotnet ef migrations remove

# Gerar script SQL
dotnet ef migrations script

# Verificar migrations pendentes (dry-run)
dotnet ef database update --dry-run
```

### Comandos Avançados
```bash
# Aplicar migration específica
dotnet ef database update NomeDaMigration

# Gerar script SQL entre duas migrations
dotnet ef migrations script MigrationInicial MigrationFinal

# Gerar script SQL com dados
dotnet ef migrations script --data
```

## 6. Troubleshooting

### Problemas Comuns

#### 1. "No project was found"
```bash
# Solução: Navegar para o diretório correto ou usar --project
cd /src/OrderService
# ou
dotnet ef migrations list --project /src/OrderService
```

#### 2. "No .NET SDKs were found"
```bash
# Solução: Verificar se está usando a imagem SDK
# Modificar Dockerfile para usar mcr.microsoft.com/dotnet/sdk:9.0
```

#### 3. "Framework not found"
```bash
# Solução: Mudar de aspnet para sdk no Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
```

### Rebuild do Container
```bash
# Parar container
docker-compose stop [nome-do-servico]

# Rebuildar imagem
docker-compose build [nome-do-servico]

# Subir novo container
docker-compose up [nome-do-servico]
```

## 7. Estrutura Final dos Containers

### Diretórios Disponíveis
- `/app/` - Arquivos compilados (DLLs) para execução da aplicação
- `/src/[NomeDoServico]/` - Código fonte completo para desenvolvimento e migrations

### Ferramentas Instaladas
- .NET SDK 9.0
- dotnet-ef (Entity Framework Core Tools)
- Todas as dependências necessárias para desenvolvimento

## 8. Próximos Passos

1. **Configurar CI/CD**: Automatizar o processo de build e deploy
2. **Health Checks**: Implementar verificações de saúde dos containers
3. **Logging**: Configurar logging centralizado
4. **Monitoring**: Implementar monitoramento dos serviços
5. **Security**: Configurar certificados e autenticação entre serviços

---

**Data de Criação**: Setembro 2024  
**Versão**: 1.0  
**Autor**: Configuração Docker para Microservices .NET Core
