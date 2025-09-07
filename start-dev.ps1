# Script para iniciar o ambiente de desenvolvimento
# Utiliza appsettings.Development.json

Write-Host "🚀 Iniciando ambiente de DESENVOLVIMENTO..." -ForegroundColor Green
Write-Host "📋 Utilizando appsettings.Development.json" -ForegroundColor Yellow

# Parar containers existentes
Write-Host "🛑 Parando containers existentes..." -ForegroundColor Red
docker-compose down

# Remover imagens antigas (opcional)
Write-Host "🗑️ Removendo imagens antigas..." -ForegroundColor Yellow
docker system prune -f

# Iniciar com ambiente de desenvolvimento
Write-Host "🏗️ Construindo e iniciando containers de desenvolvimento..." -ForegroundColor Blue
docker-compose up --build -d

Write-Host "✅ Ambiente de desenvolvimento iniciado!" -ForegroundColor Green
Write-Host "🌐 Acesse: http://localhost:5002" -ForegroundColor Cyan
Write-Host "📊 API Gateway: http://localhost:5000" -ForegroundColor Cyan
Write-Host "🔐 OAuth Services: http://localhost:5004" -ForegroundColor Cyan
Write-Host "🐰 RabbitMQ Management: http://localhost:15672" -ForegroundColor Cyan

