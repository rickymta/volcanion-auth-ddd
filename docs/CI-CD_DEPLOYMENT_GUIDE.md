# CI/CD Deployment Guide for Volcanion Auth DDD

This guide describes the comprehensive CI/CD pipeline for deploying all services in the Volcanion Auth DDD project using Docker and docker-compose.

## Architecture Overview

The application consists of the following services:
- **Volcanion Auth API**: Main .NET 8 API service
- **MySQL 8.0**: Primary database
- **Redis**: Caching and session storage
- **Elasticsearch**: Search and logging
- **Kibana**: Elasticsearch visualization (optional)
- **Prometheus**: Monitoring and metrics
- **Grafana**: Monitoring dashboards
- **Nginx**: Reverse proxy (production only)

## Prerequisites

### Development Environment
- Docker Desktop 4.0+
- Docker Compose 2.0+
- .NET 8 SDK (for local development)
- Git

### Production Environment
- Docker Swarm or Kubernetes cluster
- SSL certificates
- Environment variables management system
- Monitoring and alerting setup

## Environment Configurations

### Development (docker-compose.yml)
- Uses HTTP only
- Default passwords
- All services exposed
- Development optimizations

### Production (docker-compose.prod.yml)
- HTTPS with SSL certificates
- Environment variable-based configuration
- Resource limits and health checks
- Security hardening

## CI/CD Pipeline Steps

### 1. Source Code Management
```bash
git clone https://github.com/rickymta/volcanion-auth-ddd.git
cd volcanion-auth-ddd
git checkout develop  # or main for production
```

### 2. Environment Setup

#### Development
```bash
# Copy environment template
cp .env.example .env
# Edit .env with your configuration
```

#### Production
```bash
# Set environment variables (use your CI/CD secret management)
export DB_CONNECTION_STRING="Server=mysql;Port=3306;Database=volcanion_auth;Uid=volcanion_user;Pwd=***;"
export REDIS_CONNECTION_STRING="redis:6379"
export ELASTICSEARCH_CONNECTION_STRING="http://elasticsearch:9200"
export JWT_SECRET_KEY="your-production-secret-key-at-least-32-characters"
export DB_ROOT_PASSWORD="YourProductionPassword123!"
export DB_USER="volcanion_user"
export DB_PASSWORD="volcanion_password123!"
export ELASTICSEARCH_PASSWORD="elasticpassword123"
# Add other required environment variables
```

### 3. Build and Test

#### Unit Tests
```bash
# Run all unit tests
dotnet test tests/Volcanion.Auth.Domain.Tests/Volcanion.Auth.Domain.Tests.csproj
dotnet test tests/Volcanion.Auth.Application.Tests/Volcanion.Auth.Application.Tests.csproj
```

#### Integration Tests
```bash
# Start test dependencies
docker-compose -f docker-compose.test.yml up -d mysql redis

# Run integration tests
dotnet test tests/Volcanion.Auth.Integration.Tests/Volcanion.Auth.Integration.Tests.csproj

# Cleanup
docker-compose -f docker-compose.test.yml down
```

#### Build Docker Images
```bash
# Development
docker-compose build

# Production
docker-compose -f docker-compose.prod.yml build
```

### 4. Deployment

#### Development Deployment
```bash
# Start all services
docker-compose up -d

# Check service health
docker-compose ps
docker-compose logs volcanion-auth-api

# Access services
# API: http://localhost:5000
# Grafana: http://localhost:3000 (admin/admin)
# Kibana: http://localhost:5601
# Prometheus: http://localhost:9090
```

#### Production Deployment
```bash
# Deploy to production
docker-compose -f docker-compose.prod.yml up -d

# Verify deployment
docker-compose -f docker-compose.prod.yml ps
docker-compose -f docker-compose.prod.yml logs -f volcanion-auth-api
```

### 5. Database Migration
```bash
# Run database migrations
docker-compose exec volcanion-auth-api dotnet ef database update

# Or using MySQL scripts
docker-compose exec mysql mysql -u root -p$DB_ROOT_PASSWORD volcanion_auth < /var/backups/migration.sql
```

### 6. Health Checks and Monitoring
```bash
# Check API health
curl http://localhost:5000/health

# Check all service health
docker-compose ps

# View logs
docker-compose logs -f volcanion-auth-api
docker-compose logs -f mysql
docker-compose logs -f redis
```

### 7. Backup and Maintenance
```bash
# Database backup
docker-compose exec mysql mysqldump -u root -p$DB_ROOT_PASSWORD volcanion_auth > /var/backups/volcanion_auth_backup.sql

# Redis backup
docker-compose exec redis redis-cli --rdb /data/backup.rdb

# Log rotation and cleanup
docker system prune -f
```

## Example CI/CD Workflows

### GitHub Actions
```yaml
name: Deploy Volcanion Auth DDD

on:
  push:
    branches: [ develop, main ]
  pull_request:
    branches: [ main ]

env:
  DOCKER_REGISTRY: your-registry.com
  IMAGE_NAME: volcanion-auth-api

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
          
      - name: Restore dependencies
        run: dotnet restore
        
      - name: Build
        run: dotnet build --no-restore
        
      - name: Test
        run: |
          dotnet test tests/Volcanion.Auth.Domain.Tests --no-build --verbosity normal
          dotnet test tests/Volcanion.Auth.Application.Tests --no-build --verbosity normal

  build-and-deploy:
    needs: test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3
        
      - name: Login to Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.DOCKER_REGISTRY }}
          username: ${{ secrets.REGISTRY_USERNAME }}
          password: ${{ secrets.REGISTRY_PASSWORD }}
          
      - name: Build and push Docker image
        uses: docker/build-push-action@v5
        with:
          context: .
          push: true
          tags: |
            ${{ env.DOCKER_REGISTRY }}/${{ env.IMAGE_NAME }}:latest
            ${{ env.DOCKER_REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}
          cache-from: type=gha
          cache-to: type=gha,mode=max
          
      - name: Deploy to production
        run: |
          # Deploy using docker-compose or Kubernetes
          echo "Deploying to production..."
          # Add your deployment commands here
```

### Azure DevOps Pipeline
```yaml
trigger:
  branches:
    include:
      - main
      - develop

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dockerRegistryServiceConnection: 'your-registry-connection'
  imageRepository: 'volcanion-auth-api'
  containerRegistry: 'your-registry.azurecr.io'
  dockerfilePath: '$(Build.SourcesDirectory)/Dockerfile'
  tag: '$(Build.BuildId)'

stages:
- stage: Test
  displayName: Test stage
  jobs:
  - job: Test
    displayName: Test
    steps:
    - task: DotNetCoreCLI@2
      displayName: Restore
      inputs:
        command: 'restore'
        projects: '**/*.csproj'
        
    - task: DotNetCoreCLI@2
      displayName: Build
      inputs:
        command: 'build'
        projects: '**/*.csproj'
        arguments: '--configuration $(buildConfiguration)'
        
    - task: DotNetCoreCLI@2
      displayName: Test
      inputs:
        command: 'test'
        projects: '**/tests/**/*.csproj'
        arguments: '--configuration $(buildConfiguration) --collect "Code coverage"'

- stage: Build
  displayName: Build and push stage
  dependsOn: Test
  jobs:
  - job: Build
    displayName: Build
    steps:
    - task: Docker@2
      displayName: Build and push an image to container registry
      inputs:
        command: buildAndPush
        repository: $(imageRepository)
        dockerfile: $(dockerfilePath)
        containerRegistry: $(dockerRegistryServiceConnection)
        tags: |
          $(tag)
          latest

- stage: Deploy
  displayName: Deploy stage
  dependsOn: Build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: Deploy
    displayName: Deploy
    environment: 'production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: DockerCompose@0
            displayName: Run docker-compose
            inputs:
              action: 'Run services'
              dockerComposeFile: 'docker-compose.prod.yml'
              buildImages: false
```

## Security Considerations

### Production Security Checklist
- [ ] Use strong, unique passwords for all services
- [ ] Enable SSL/TLS encryption
- [ ] Configure proper firewall rules
- [ ] Use non-root users in containers
- [ ] Regularly update base images
- [ ] Implement rate limiting
- [ ] Set up monitoring and alerting
- [ ] Use secrets management
- [ ] Enable audit logging
- [ ] Configure backup and disaster recovery

### Environment Variables Security
```bash
# Use a secure secrets management system
# Never commit secrets to version control
# Rotate secrets regularly
# Use least privilege principle
```

## Monitoring and Observability

### Health Endpoints
- API Health: `http://localhost:5000/health`
- Prometheus Metrics: `http://localhost:9090`
- Grafana Dashboards: `http://localhost:3000`
- Elasticsearch Health: `http://localhost:9200/_cluster/health`

### Key Metrics to Monitor
- API response times and error rates
- Database connection pool usage
- Redis memory usage and hit rates
- Elasticsearch cluster health
- Container resource usage (CPU, Memory, Disk)
- Business metrics (authentication success/failure rates)

## Troubleshooting

### Common Issues
1. **Container startup failures**: Check logs with `docker-compose logs [service]`
2. **Database connection issues**: Verify connection strings and network connectivity
3. **Memory issues**: Adjust resource limits in docker-compose files
4. **SSL certificate problems**: Ensure certificates are properly mounted and configured

### Useful Commands
```bash
# View all container logs
docker-compose logs -f

# Restart specific service
docker-compose restart volcanion-auth-api

# Check container resource usage
docker stats

# Execute commands in running container
docker-compose exec volcanion-auth-api bash

# View container health status
docker-compose ps
```

## Scaling and Performance

### Horizontal Scaling
```yaml
# Scale API instances
docker-compose up -d --scale volcanion-auth-api=3

# Use load balancer (nginx) to distribute traffic
# Configure Redis for session sharing
# Use read replicas for database scaling
```

### Performance Optimization
- Enable Redis caching
- Configure database connection pooling
- Use CDN for static assets
- Implement API response caching
- Optimize database queries and indexes

---

For additional support or customization, refer to the project documentation or contact the development team.
