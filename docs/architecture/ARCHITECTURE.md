# Arquitetura do Sistema AgroSolutions

## Visão Geral da Arquitetura
<img width="978" height="590" alt="Visaogeral" src="https://github.com/user-attachments/assets/a599a047-1a5e-4a07-aacd-53a7afd420d6" />

## Fluxo de Dados Detalhado
<img width="1048" height="441" alt="Fluxodados" src="https://github.com/user-attachments/assets/59d961ae-7c7b-4d31-9e82-a1fa8feac6f2" />

## Arquitetura de Cada Microserviço
<img width="1247" height="315" alt="ArquiteturaCadaMicroservico" src="https://github.com/user-attachments/assets/15759a83-43d9-43a8-aa12-0d00c07b35ac" />

## Comunicação entre Microserviços
<img width="765" height="483" alt="comunicacaoMicroservicos" src="https://github.com/user-attachments/assets/fd55c062-c23c-4688-9d2e-85e6d8d89a79" />

## Motor de Alertas - Regras de Negócio
<img width="716" height="679" alt="MotorDeAlertasRegrasDenegocio" src="https://github.com/user-attachments/assets/ccd10ceb-ffe3-4ba1-a5f3-a72adba3aa73" />

## Stack Tecnológica
<img width="1050" height="499" alt="StackTecnologica" src="https://github.com/user-attachments/assets/260c7db6-1e24-440c-82ec-03e099e70ccc" />

## CI/CD Pipeline
<img width="956" height="573" alt="CICDPipeline" src="https://github.com/user-attachments/assets/e1a9b753-7400-4540-b20d-fde61f4280b8" />


## Decisões Arquiteturais

### Por que Microserviços?

- **Escalabilidade independente**: Cada serviço pode escalar conforme demanda
- **Isolamento de falhas**: Problema em um serviço não afeta os outros
- **Tecnologias específicas**: Cada serviço pode usar stack adequada
- **Deploy independente**: Atualizações sem afetar todo sistema

### Por que RabbitMQ?

- **Desacoplamento**: Serviços não precisam conhecer uns aos outros
- **Resiliência**: Mensagens persistidas mesmo se consumer estiver offline
- **Processamento assíncrono**: Alertas gerados sem bloquear ingestão

### Por que PostgreSQL?

- **ACID compliant**: Garantias transacionais
- **Open source**: Sem custos de licença
- **Extensível**: Suporte a tipos complexos
- **Performance**: Ótimo para cargas OLTP

### Por que Clean Architecture?

- **Testabilidade**: Camadas desacopladas facilitam testes
- **Manutenibilidade**: Código organizado e fácil de entender
- **Independência**: Domain não depende de frameworks
- **Flexibilidade**: Fácil trocar implementações de infraestrutura

### Por que Blazor WebAssembly?

- **C# Full Stack**: Mesma linguagem no backend e frontend
- **Performance**: Executa no browser via WebAssembly
- **Componentização**: Reutilização de código
- **TypeScript-free**: Sem necessidade de aprender JavaScript
