# AgroSolutions - Plataforma IoT para Agricultura de Precisão

[![CI/CD Pipeline](https://github.com/devjoaomelo/fiap-AgroSolutions/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/devjoaomelo/fiap-AgroSolutions/actions)

Sistema de monitoramento agrícola baseado em IoT para otimização de recursos hídricos, aumento de produtividade e sustentabilidade no campo.

## Índice

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Tecnologias](#tecnologias)
- [Como Executar](#como-executar)
- [Testes](#testes)
- [CI/CD](#cicd)
- [API Documentation](#api-documentation)

## Visão Geral

A AgroSolutions é uma plataforma de agricultura 4.0 que coleta dados de sensores IoT no campo e gera alertas inteligentes para os produtores rurais, auxiliando na tomada de decisões sobre irrigação, controle de temperatura e gestão de precipitação.

### Problema Resolvido

- **Desperdício de recursos**: Irrigação sem dados em tempo real
- **Baixa produtividade**: Decisões baseadas apenas na experiência
- **Riscos climáticos**: Sem monitoramento contínuo das condições

### Solução

Sistema distribuído de microserviços que:
- Ingere dados de sensores em tempo real
- Processa regras de negócio automaticamente
- Gera alertas inteligentes via mensageria
- Fornece APIs RESTful para integração

## Arquitetura

### Microserviços

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Identity  │    │  Property   │    │  Ingestion  │    │   Alerts    │
│   Service   │    │   Service   │    │   Service   │    │   Service   │
│             │    │             │    │             │    │             │
│  - Auth     │    │  - Props    │    │  - Sensors  │    │  - Rules    │
│  - Users    │    │  - Fields   │    │  - Data     │    │  - Notify   │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
      │                   │                   │                   │
      └───────────────────┴───────────────────┴───────────────────┘
                                  │
                    ┌─────────────┴─────────────┐
                    │                           │
              ┌─────▼─────┐              ┌─────▼─────┐
              │ RabbitMQ  │              │PostgreSQL │
              │ (Message  │              │(Database) │
              │  Broker)  │              │           │
              └───────────┘              └───────────┘
```
[Veja Mais](https://github.com/devjoaomelo/fiap-AgroSolutions/blob/main/docs/architecture/ARCHITECTURE.md)

### Fluxo de Dados

1. **Autenticação**: Produtor faz login no Identity Service
2. **Cadastro**: Registra propriedades e talhões no Property Service
3. **Ingestão**: Sensores enviam dados para Ingestion Service
4. **Mensageria**: Ingestion publica evento no RabbitMQ
5. **Processamento**: Alerts Service consome evento e aplica regras
6. **Alertas**: Sistema gera alertas automáticos se necessário

### Regras de Negócio (Motor de Alertas)

| Condição | Tipo de Alerta | Severidade |
|----------|----------------|------------|
| Umidade do solo < 30% | Seca | Alta |
| Temperatura > 35°C | Alta Temperatura | Média |
| Precipitação > 50mm | Chuva Forte | Alta |

## Funcionalidades

### Identity Service
- ✅ Registro de usuários com validação
- ✅ Login com JWT
- ✅ Hash de senhas com BCrypt
- ✅ Primeiro usuário vira Admin automaticamente

### Property Service
- ✅ Cadastro de propriedades rurais
- ✅ Cadastro de talhões (fields)
- ✅ Consulta por usuário
- ✅ Relacionamento propriedade → talhões

### Ingestion Service
- ✅ Recebimento de dados de sensores
- ✅ Validação de dados (ranges)
- ✅ Publicação de eventos no RabbitMQ
- ✅ Persistência em banco de dados

### Alerts Service
- ✅ Consumo automático de eventos
- ✅ Motor de regras de negócio
- ✅ Prevenção de alertas duplicados
- ✅ Consulta de alertas por talhão
- ✅ Resolução de alertas

## Tecnologias

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core** - Web APIs
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados relacional
- **MassTransit** - Abstração para RabbitMQ
- **BCrypt.Net** - Hash de senhas

### Mensageria
- **RabbitMQ** - Message broker

### Observabilidade
- **Seq** - Logs centralizados
- **Prometheus** - Métricas
- **Grafana** - Dashboards

### DevOps
- **Docker** - Containerização
- **Docker Compose** - Orquestração local
- **GitHub Actions** - CI/CD
- **Kubernetes** - Orquestração em produção (planejado)

### Testes
- **xUnit** - Framework de testes
- **Moq** - Mocking
- **FluentAssertions** - Assertions

## Como Executar

### Pré-requisitos

- Docker Desktop
- .NET 8 SDK (para desenvolvimento local)

### Opção 1: Docker Compose (Recomendado)

```bash
# Clone o repositório
git clone https://github.com/devjoaomelo/AgroSolutions.git
cd AgroSolutions

# Suba toda a infraestrutura
docker compose up -d

# Aguarde ~30 segundos para tudo inicializar
```

**Serviços disponíveis:**
- Identity API: http://localhost:5001/swagger
- Property API: http://localhost:5002/swagger
- Ingestion API: http://localhost:5003/swagger
- Alerts API: http://localhost:5004/swagger
- RabbitMQ Management: http://localhost:15672 (guest/guest)
- Seq Logs: http://localhost:5341
- Prometheus: http://localhost:9090
- Grafana: http://localhost:3000 (admin/admin)

### Opção 2: Desenvolvimento Local

```bash
# Suba apenas a infraestrutura
docker compose up -d rabbitmq postgres-identity postgres-property postgres-ingestion postgres-alerts seq prometheus grafana

# Rode cada serviço separadamente
cd src/services/Identity/AgroSolutions.Identity.Api
dotnet run

# Em outros terminais...
cd src/services/Property/AgroSolutions.Property.Api
dotnet run

# E assim por diante...
```

## Testes

### Executar todos os testes

```bash
dotnet test
```

### Executar testes por serviço

```bash
# Identity
dotnet test src/services/Identity/AgroSolutions.Identity.Tests/

# Ingestion
dotnet test src/services/Ingestion/AgroSolutions.Ingestion.Tests/

# Alerts
dotnet test src/services/Alerts/AgroSolutions.Alerts.Tests/
```

### Cobertura de Testes

- **Identity Service**: 6 testes - Validações, registro, hash
- **Ingestion Service**: 8 testes - Validações, publicação de eventos
- **Alerts Service**: 6 testes - Regras de negócio, processamento

## CI/CD

Pipeline automatizado com GitHub Actions:

### Jobs

1. **Test** - Roda todos os testes unitários
2. **Build** - Cria imagens Docker dos 4 serviços
3. **Push** - Publica imagens no Docker Hub (branch main)

### Triggers

- Push em `main`, `master`, `develop`
- Pull requests para `main`, `master`

### Visualizar Pipeline

Acesse: `https://github.com/devjoaomelo/fiap-AgroSolutions/actions`

## API Documentation

### Fluxo Completo de Uso

#### 1. Registrar Usuário
```bash
POST http://localhost:5001/api/auth/register
{
  "name": "João Silva",
  "email": "joao@agro.com",
  "password": "Senha@123"
}
```

#### 2. Login
```bash
POST http://localhost:5001/api/auth/login
{
  "email": "joao@agro.com",
  "password": "Senha@123"
}
```

#### 3. Cadastrar Propriedade
```bash
POST http://localhost:5002/api/properties
{
  "userId": "guid-do-usuario",
  "name": "Fazenda São José",
  "location": "Campinas, SP",
  "totalArea": 500.0
}
```

#### 4. Cadastrar Talhão
```bash
POST http://localhost:5002/api/fields
{
  "ruralPropertyId": "guid-da-propriedade",
  "name": "Talhão 1",
  "culture": "Soja",
  "area": 50.0
}
```

#### 5. Enviar Dados do Sensor
```bash
POST http://localhost:5003/api/sensordata
{
  "fieldId": "guid-do-talhao",
  "soilMoisture": 20.0,
  "temperature": 38.0,
  "precipitation": 60.0
}
```

#### 6. Consultar Alertas (automático!)
```bash
GET http://localhost:5004/api/alerts?fieldId=guid-do-talhao
```

**Resultado esperado**: 3 alertas criados automaticamente via RabbitMQ
- Alerta de Seca (umidade < 30%)
- Alerta de Alta Temperatura (temp > 35°C)
- Alerta de Chuva Forte (precip > 50mm)

## Monitoramento

### Logs (Seq)
- Acesse: http://localhost:5341
- Busque por: `@Level = 'Error'` ou `@Level = 'Information'`
- Filtre por serviço: `ServiceName = 'agrosolutions-alerts-api'`

### Métricas (Prometheus + Grafana)
- Prometheus: http://localhost:9090
- Grafana: http://localhost:3000
- Dashboards pré-configurados para cada serviço

### RabbitMQ
- Management UI: http://localhost:15672
- Monitore filas, throughput e consumers

## Próximos Passos

- [ ] Manifestos Kubernetes
- [ ] Dashboard Blazor
- [ ] Integração com API de clima
- [ ] Notificações por email/SMS
- [ ] Machine Learning para previsões

## Autor

Desenvolvido por JOÃO MELO como parte do Hackathon 8NETT - Pós-Graduação em Arquitetura de Software .NET

## Licença

Este projeto é parte de um trabalho acadêmico.
