# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WorkerCatalog/WorkerCatalog.csproj", "WorkerCatalog/"]
RUN dotnet restore "./WorkerCatalog/WorkerCatalog.csproj"
COPY . .
WORKDIR "/src/WorkerCatalog"
RUN dotnet build "./WorkerCatalog.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WorkerCatalog.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final  
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final  
WORKDIR /app

# Copiar arquivos publicados (DLLs)
COPY --from=publish /app/publish .

# Copiar código fonte da fase build para uma pasta separada: somente utilizar para permitir o uso do comando 'dotnet ef' no container
#COPY --from=build /src/WorkerCatalog /src/WorkerCatalog

# Instalar dotnet-ef
#RUN dotnet tool install --global dotnet-ef
#ENV PATH="${PATH}:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "WorkerCatalog.dll"]