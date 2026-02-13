FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["AgroSolutions.Identity.Api/AgroSolutions.Identity.Api.csproj", "AgroSolutions.Identity.Api/"]
COPY ["AgroSolutions.Identity.Application/AgroSolutions.Identity.Application.csproj", "AgroSolutions.Identity.Application/"]
COPY ["AgroSolutions.Identity.Domain/AgroSolutions.Identity.Domain.csproj", "AgroSolutions.Identity.Domain/"]
COPY ["AgroSolutions.Identity.Infrastructure/AgroSolutions.Identity.Infrastructure.csproj", "AgroSolutions.Identity.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "AgroSolutions.Identity.Api/AgroSolutions.Identity.Api.csproj"

# Copiar código fonte
COPY . .

# Build
WORKDIR "/src/AgroSolutions.Identity.Api"
RUN dotnet build "AgroSolutions.Identity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgroSolutions.Identity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgroSolutions.Identity.Api.dll"]
