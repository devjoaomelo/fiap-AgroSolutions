FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AgroSolutions.Alerts.Api/AgroSolutions.Alerts.Api.csproj", "AgroSolutions.Alerts.Api/"]
COPY ["AgroSolutions.Alerts.Application/AgroSolutions.Alerts.Application.csproj", "AgroSolutions.Alerts.Application/"]
COPY ["AgroSolutions.Alerts.Domain/AgroSolutions.Alerts.Domain.csproj", "AgroSolutions.Alerts.Domain/"]
COPY ["AgroSolutions.Alerts.Infrastructure/AgroSolutions.Alerts.Infrastructure.csproj", "AgroSolutions.Alerts.Infrastructure/"]

RUN dotnet restore "AgroSolutions.Alerts.Api/AgroSolutions.Alerts.Api.csproj"

COPY . .

WORKDIR "/src/AgroSolutions.Alerts.Api"
RUN dotnet build "AgroSolutions.Alerts.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgroSolutions.Alerts.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgroSolutions.Alerts.Api.dll"]
