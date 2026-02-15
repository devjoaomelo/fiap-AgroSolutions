FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["AgroSolutions.Ingestion.Api/AgroSolutions.Ingestion.Api.csproj", "AgroSolutions.Ingestion.Api/"]
COPY ["AgroSolutions.Ingestion.Application/AgroSolutions.Ingestion.Application.csproj", "AgroSolutions.Ingestion.Application/"]
COPY ["AgroSolutions.Ingestion.Domain/AgroSolutions.Ingestion.Domain.csproj", "AgroSolutions.Ingestion.Domain/"]
COPY ["AgroSolutions.Ingestion.Infrastructure/AgroSolutions.Ingestion.Infrastructure.csproj", "AgroSolutions.Ingestion.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "AgroSolutions.Ingestion.Api/AgroSolutions.Ingestion.Api.csproj"

# Copiar código fonte
COPY . .

# Build
WORKDIR "/src/AgroSolutions.Ingestion.Api"
RUN dotnet build "AgroSolutions.Ingestion.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgroSolutions.Ingestion.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgroSolutions.Ingestion.Api.dll"]
