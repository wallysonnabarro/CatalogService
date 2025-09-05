# Instruções para Criar as Migrations dos Logs

## Comandos para Executar

### 1. CatalogService
```bash
cd CatalogService
dotnet ef migrations add AddApplicationLogs
dotnet ef database update
```

### 2. OrderService
```bash
cd OrderService
dotnet ef migrations add AddApplicationLogs
dotnet ef database update
```

### 3. OAuthServices
```bash
cd OAuthServices
dotnet ef migrations add AddApplicationLogs
dotnet ef database update
```

### 4. ApiGateway
```bash
cd ApiGateway
dotnet ef migrations add AddApplicationLogs
dotnet ef database update
```

## Estrutura da Tabela ApplicationLogs

A tabela será criada com os seguintes campos:

```sql
CREATE TABLE [ApplicationLogs] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CorrelationId] nvarchar(50) NOT NULL,
    [ServiceName] nvarchar(100) NOT NULL,
    [LogLevel] nvarchar(20) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Exception] nvarchar(max) NULL,
    [Timestamp] datetime2 NOT NULL,
    [UserId] nvarchar(100) NULL,
    [RequestPath] nvarchar(500) NULL,
    [HttpMethod] nvarchar(10) NULL,
    [Duration] int NULL,
    CONSTRAINT [PK_ApplicationLogs] PRIMARY KEY ([Id])
);
```

## Verificação

Após executar as migrations, você pode verificar se as tabelas foram criadas:

```sql
-- Para CatalogService
SELECT * FROM ApplicationLogs WHERE ServiceName = 'CatalogService';

-- Para OrderService  
SELECT * FROM ApplicationLogs WHERE ServiceName = 'OrderService';

-- Para OAuthServices
SELECT * FROM ApplicationLogs WHERE ServiceName = 'OAuthServices';

-- Para ApiGateway
SELECT * FROM ApplicationLogs WHERE ServiceName = 'ApiGateway';
```

## Teste dos Logs

Após executar as migrations, faça uma requisição para qualquer endpoint e verifique se os logs estão sendo salvos no banco:

```sql
SELECT TOP 10 * FROM ApplicationLogs 
ORDER BY Timestamp DESC;
```
