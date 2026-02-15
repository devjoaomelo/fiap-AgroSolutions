FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["AgroSolutions.Property.Api/AgroSolutions.Property.Api.csproj", "AgroSolutions.Property.Api/"]
COPY ["AgroSolutions.Property.Application/AgroSolutions.Property.Application.csproj", "AgroSolutions.Property.Application/"]
COPY ["AgroSolutions.Property.Domain/AgroSolutions.Property.Domain.csproj", "AgroSolutions.Property.Domain/"]
COPY ["AgroSolutions.Property.Infrastructure/AgroSolutions.Property.Infrastructure.csproj", "AgroSolutions.Property.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "AgroSolutions.Property.Api/AgroSolutions.Property.Api.csproj"

# Copiar código fonte
COPY . .

# Build
WORKDIR "/src/AgroSolutions.Property.Api"
RUN dotnet build "AgroSolutions.Property.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgroSolutions.Property.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgroSolutions.Property.Api.dll"]
