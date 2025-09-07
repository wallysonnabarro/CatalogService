# Script para iniciar o ambiente de produção
# Utiliza appsettings.Production.json

Write-Host "🚀 Iniciando ambiente de PRODUÇÃO..." -ForegroundColor Green
Write-Host "📋 Utilizando appsettings.Production.json" -ForegroundColor Yellow

# Parar containers existentes
Write-Host "🛑 Parando containers existentes..." -ForegroundColor Red
docker-compose -f docker-compose.prod.yml down

# Remover imagens antigas (opcional)
Write-Host "🗑️ Removendo imagens antigas..." -ForegroundColor Yellow
docker system prune -f

# Iniciar com ambiente de produção
Write-Host "🏗️ Construindo e iniciando containers de produção..." -ForegroundColor Blue
docker-compose -f docker-compose.prod.yml up --build -d

Write-Host "✅ Ambiente de produção iniciado!" -ForegroundColor Green
Write-Host "🌐 Acesse: http://localhost:5003" -ForegroundColor Cyan
Write-Host "📊 API Gateway: http://localhost:5001" -ForegroundColor Cyan
Write-Host "🔐 OAuth Services: http://localhost:5005" -ForegroundColor Cyan
Write-Host "🐰 RabbitMQ Management: http://localhost:15672" -ForegroundColor Cyan

