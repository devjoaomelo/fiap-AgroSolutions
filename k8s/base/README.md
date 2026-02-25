# Kubernetes Deployment - AgroSolutions

Este diretório contém os manifestos Kubernetes para deploy da plataforma AgroSolutions.

## Pré-requisitos

- Kubernetes cluster (minikube, kind, ou cloud provider)
- kubectl instalado e configurado
- Ingress Controller (NGINX) instalado no cluster

## Estrutura
```
k8s/
└── base/
    ├── namespace.yaml              # Namespace agrosolutions
    ├── configmap.yaml              # Configurações não-sensíveis
    ├── secrets.yaml                # Senhas e chaves (base64)
    ├── postgres-identity.yaml      # Database Identity
    ├── postgres-property.yaml      # Database Property
    ├── postgres-ingestion.yaml     # Database Ingestion
    ├── postgres-alerts.yaml        # Database Alerts
    ├── rabbitmq.yaml               # Message Broker
    ├── identity-api.yaml           # Identity Service
    ├── property-api.yaml           # Property Service
    ├── ingestion-api.yaml          # Ingestion Service
    ├── alerts-api.yaml             # Alerts Service
    ├── ingress.yaml                # Ingress (NGINX)
    └── kustomization.yaml          # Kustomize config
```

## Deploy Completo

### Opção 1: Usando kubectl
```bash
# Apply todos os manifestos de uma vez
kubectl apply -k k8s/base/

# Verificar status
kubectl get all -n agrosolutions
kubectl get pvc -n agrosolutions
kubectl get ingress -n agrosolutions
```

### Opção 2: Deploy Manual (passo a passo)
```bash
# 1. Criar namespace
kubectl apply -f k8s/base/namespace.yaml

# 2. Aplicar configs e secrets
kubectl apply -f k8s/base/configmap.yaml
kubectl apply -f k8s/base/secrets.yaml

# 3. Deploy databases (aguarde ficarem prontos)
kubectl apply -f k8s/base/postgres-identity.yaml
kubectl apply -f k8s/base/postgres-property.yaml
kubectl apply -f k8s/base/postgres-ingestion.yaml
kubectl apply -f k8s/base/postgres-alerts.yaml

# Aguarde os databases ficarem prontos
kubectl wait --for=condition=ready pod -l app=postgres-identity -n agrosolutions --timeout=120s
kubectl wait --for=condition=ready pod -l app=postgres-property -n agrosolutions --timeout=120s
kubectl wait --for=condition=ready pod -l app=postgres-ingestion -n agrosolutions --timeout=120s
kubectl wait --for=condition=ready pod -l app=postgres-alerts -n agrosolutions --timeout=120s

# 4. Deploy RabbitMQ
kubectl apply -f k8s/base/rabbitmq.yaml
kubectl wait --for=condition=ready pod -l app=rabbitmq -n agrosolutions --timeout=120s

# 5. Deploy APIs
kubectl apply -f k8s/base/identity-api.yaml
kubectl apply -f k8s/base/property-api.yaml
kubectl apply -f k8s/base/ingestion-api.yaml
kubectl apply -f k8s/base/alerts-api.yaml

# 6. Deploy Ingress
kubectl apply -f k8s/base/ingress.yaml
```

## Configurar Hosts Local

Para acessar via `agrosolutions.local`, adicione ao `/etc/hosts` (Linux/Mac) ou `C:\Windows\System32\drivers\etc\hosts` (Windows):
```
127.0.0.1 agrosolutions.local
```

Se estiver usando **minikube**:
```bash
minikube ip  # anote o IP
# Adicione ao hosts: <MINIKUBE_IP> agrosolutions.local
```

## Acessar as APIs

Após o deploy, as APIs estarão disponíveis em:

- Identity API: http://agrosolutions.local/api/identity/swagger
- Property API: http://agrosolutions.local/api/property/swagger
- Ingestion API: http://agrosolutions.local/api/ingestion/swagger
- Alerts API: http://agrosolutions.local/api/alerts/swagger

## Verificar Status
```bash
# Pods
kubectl get pods -n agrosolutions

# Services
kubectl get svc -n agrosolutions

# Persistent Volume Claims
kubectl get pvc -n agrosolutions

# Logs de um pod específico
kubectl logs -f deployment/identity-api -n agrosolutions
kubectl logs -f deployment/alerts-api -n agrosolutions

# Descrever um pod com problemas
kubectl describe pod <pod-name> -n agrosolutions
```

## Escalar Serviços
```bash
# Aumentar replicas da Identity API
kubectl scale deployment identity-api -n agrosolutions --replicas=3

# Diminuir replicas da Property API
kubectl scale deployment property-api -n agrosolutions --replicas=1
```

## Port-Forward (alternativa ao Ingress)

Se não tiver Ingress Controller instalado:
```bash
# Identity API
kubectl port-forward svc/identity-api-service 5001:80 -n agrosolutions

# Property API
kubectl port-forward svc/property-api-service 5002:80 -n agrosolutions

# Ingestion API
kubectl port-forward svc/ingestion-api-service 5003:80 -n agrosolutions

# Alerts API
kubectl port-forward svc/alerts-api-service 5004:80 -n agrosolutions

# RabbitMQ Management
kubectl port-forward svc/rabbitmq-service 15672:15672 -n agrosolutions
```

## Migrações de Banco de Dados

Execute as migrations dentro dos pods:
```bash
# Identity
kubectl exec -it deployment/identity-api -n agrosolutions -- dotnet ef database update

# Ou conecte diretamente no PostgreSQL
kubectl exec -it deployment/postgres-identity -n agrosolutions -- psql -U identity_user -d identity_db
```

## Deletar Tudo
```bash
# Usando kustomize
kubectl delete -k k8s/base/

# Ou manualmente
kubectl delete namespace agrosolutions
```

## Troubleshooting

### Pods não iniciam
```bash
# Ver eventos
kubectl get events -n agrosolutions --sort-by='.lastTimestamp'

# Descrever pod com problema
kubectl describe pod <pod-name> -n agrosolutions

# Ver logs
kubectl logs <pod-name> -n agrosolutions
```

### Erro de conexão com banco
```bash
# Verificar se os databases estão rodando
kubectl get pods -n agrosolutions | grep postgres

# Testar conexão
kubectl exec -it deployment/postgres-identity -n agrosolutions -- psql -U identity_user -d identity_db -c "SELECT 1"
```

### ImagePullBackOff

Certifique-se que as imagens existem no Docker Hub:
- `devjoaomelo/agrosolutions-identity:latest`
- `devjoaomelo/agrosolutions-property:latest`
- `devjoaomelo/agrosolutions-ingestion:latest`
- `devjoaomelo/agrosolutions-alerts:latest`

Se necessário, ajuste os nomes das imagens nos arquivos `*-api.yaml`.

## Segurança - IMPORTANTE

**ANTES DE DEPLOY EM PRODUÇÃO:**

1. **Trocar secrets!** Os valores em `secrets.yaml` são exemplos. Gere novos:
```bash
echo -n "sua-senha-forte" | base64
```

2. **Usar Secrets Manager** em produção (AWS Secrets Manager, Azure Key Vault, etc)

3. **Habilitar TLS/SSL** no Ingress com certificados válidos

4. **Network Policies** para restringir comunicação entre pods

5. **Resource Limits** adequados para seu cluster

## Monitoramento

Para adicionar observabilidade completa:
```bash
# Prometheus + Grafana (usando Helm)
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm install prometheus prometheus-community/kube-prometheus-stack -n monitoring --create-namespace
```

## Recursos

- [Documentação Kubernetes](https://kubernetes.io/docs/)
- [Kustomize](https://kustomize.io/)
- [NGINX Ingress Controller](https://kubernetes.github.io/ingress-nginx/)