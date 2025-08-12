# 🔐 Volcanion Auth DDD

[![CI/CD Pipeline](https://github.com/rickymta/volcanion-auth-ddd/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/rickymta/volcanion-auth-ddd/actions/workflows/ci-cd.yml)
[![Code Coverage](https://codecov.io/gh/rickymta/volcanion-auth-ddd/branch/main/graph/badge.svg)](https://codecov.io/gh/rickymta/volcanion-auth-ddd)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)

A robust, scalable authentication and authorization system built with **Domain-Driven Design (DDD)** principles using .NET 8, Entity Framework Core, and JWT authentication.

## 🚀 Features

### 🔐 Authentication & Authorization
- **JWT-based authentication** with refresh tokens
- **Role-based access control (RBAC)**
- **Permission-based authorization**
- **User registration and login**
- **Password reset functionality**
- **Email and phone verification**

### 🏗️ Architecture
- **Domain-Driven Design (DDD)** implementation
- **Clean Architecture** with clear separation of concerns
- **CQRS pattern** with MediatR
- **Event-driven architecture** with domain events
- **Repository pattern** with Unit of Work
- **Value objects** and **Aggregates**

### �️ Technical Stack
- **.NET 8** - Latest framework version
- **Entity Framework Core** - ORM with SQL Server
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **MediatR** - CQRS and mediator pattern
- **xUnit** - Unit testing framework
- **Docker** - Containerization
- **Redis** - Caching (optional)

### � Quality & Monitoring
- **Comprehensive unit tests** (169 tests, 98%+ coverage)
- **Integration tests** with in-memory database
- **CI/CD pipeline** with GitHub Actions
- **Code quality analysis** with SonarCloud
- **Health checks** and monitoring
- **API documentation** with Swagger/OpenAPI

## � Project Structure

```
src/
├── Volcanion.Auth.Domain/          # Domain layer (entities, value objects, domain services)
├── Volcanion.Auth.Application/     # Application layer (use cases, DTOs, interfaces)
├── Volcanion.Auth.Infrastructure/  # Infrastructure layer (repositories, external services)
└── Volcanion.Auth.Presentation/    # Presentation layer (controllers, middleware)

tests/
├── Volcanion.Auth.Domain.Tests/        # Domain unit tests
├── Volcanion.Auth.Application.Tests/   # Application unit tests
└── Volcanion.Auth.Integration.Tests/   # Integration tests

docs/
├── postman/                        # Postman collections
├── api/                            # API documentation
└── architecture/                   # Architecture diagrams
```

## 🚀 Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (optional)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) or Docker SQL Server

### 1. Clone the Repository
```bash
git clone https://github.com/rickymta/volcanion-auth-ddd.git
cd volcanion-auth-ddd
```

### 2. Run with Docker (Recommended)
```bash
# Start all services
docker-compose up -d

# The API will be available at:
# HTTP: http://localhost:5000
# HTTPS: https://localhost:5001
```

### 3. Run Locally
```bash
# Restore dependencies
dotnet restore

# Update database
dotnet ef database update --project src/Volcanion.Auth.Infrastructure --startup-project src/Volcanion.Auth.Presentation

# Run the application
dotnet run --project src/Volcanion.Auth.Presentation
```

### 4. Run Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults

# Generate coverage report
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:"Html"
```

## 🔧 Configuration

### Environment Variables
```bash
# Database
ConnectionStrings__DefaultConnection="Server=localhost;Database=VolcanionAuthDb;Trusted_Connection=true;"

# JWT Settings
JWT__SecretKey="your-super-secret-jwt-key-that-is-at-least-32-characters-long"
JWT__Issuer="VolcanionAuth"
JWT__Audience="VolcanionAuthUsers"
JWT__ExpiryInMinutes=60

# Redis (optional)
Redis__ConnectionString="localhost:6379"
Redis__Password="redispassword"

# Email (optional)
Email__SmtpServer="smtp.gmail.com"
Email__SmtpPort=587
Email__Username="your-email@gmail.com"
Email__Password="your-app-password"
```

### appsettings.json Example
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=VolcanionAuthDb;Trusted_Connection=true;"
  },
  "JWT": {
    "SecretKey": "your-super-secret-jwt-key-that-is-at-least-32-characters-long",
    "Issuer": "VolcanionAuth",
    "Audience": "VolcanionAuthUsers",
    "ExpiryInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 📚 API Documentation

### 🔗 Endpoints Overview

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/api/auth/register` | Register new user | ❌ |
| `POST` | `/api/auth/login` | User login | ❌ |
| `POST` | `/api/auth/refresh` | Refresh JWT token | ❌ |
| `POST` | `/api/auth/logout` | User logout | ✅ |
| `GET` | `/api/users/me` | Get current user | ✅ |
| `PUT` | `/api/users/me` | Update user profile | ✅ |
| `POST` | `/api/users/change-password` | Change password | ✅ |
| `GET` | `/api/roles` | Get all roles | ✅ |
| `POST` | `/api/roles` | Create new role | ✅ |
| `GET` | `/api/permissions` | Get all permissions | ✅ |
| `GET` | `/health` | Health check | ❌ |

### 📄 Swagger Documentation
When running the application, visit:
- Development: `http://localhost:5000/swagger`
- Production: `https://your-domain.com/swagger`

### 📮 Postman Collection
Import the Postman collection from `docs/postman/` directory:
1. **Collection**: `Volcanion-Auth-API.postman_collection.json`
2. **Environment**: `Development.postman_environment.json`

## 🧪 Testing

### Test Coverage
- **Domain Tests**: 155 tests covering entities, value objects, and domain services
- **Application Tests**: Tests for use cases and application services
- **Integration Tests**: End-to-end API testing
- **Total Coverage**: 98%+ code coverage

### Running Specific Tests
```bash
# Domain tests only
dotnet test tests/Volcanion.Auth.Domain.Tests

# Application tests only
dotnet test tests/Volcanion.Auth.Application.Tests

# Integration tests only
dotnet test tests/Volcanion.Auth.Integration.Tests
```

## 🔄 CI/CD Pipeline

### GitHub Actions Workflows
1. **CI/CD Pipeline** (`.github/workflows/ci-cd.yml`)
   - ✅ Run tests and generate coverage
   - 🔍 Security scanning
   - 🐳 Build and push Docker images
   - 🚀 Deploy to staging/production

2. **Code Quality** (`.github/workflows/code-quality.yml`)
   - 📊 SonarCloud analysis
   - 🔍 CodeQL security analysis

3. **PR Validation** (`.github/workflows/pr-validation.yml`)
   - ✅ Validate pull requests
   - 🎨 Check code formatting
   - 📝 Lint PR titles

### Deployment
```bash
# Deploy with Docker
docker-compose -f docker-compose.prod.yml up -d

# Deploy to Kubernetes
kubectl apply -f k8s/
```

## 🏗️ Architecture

### Domain-Driven Design
The project follows DDD principles with:
- **Entities**: `User`, `Role`, `Permission`, `UserSession`
- **Value Objects**: `Email`, `Password`, `PhoneNumber`
- **Aggregates**: `UserAggregate` with consistency boundaries
- **Domain Services**: `IAuthenticationService`, `IAuthorizationService`
- **Domain Events**: `UserRegisteredEvent`, `UserLoggedInEvent`

### Clean Architecture Layers
```
┌─────────────────────────────────────┐
│           Presentation              │ ← Controllers, Middleware
├─────────────────────────────────────┤
│           Application               │ ← Use Cases, DTOs, Interfaces
├─────────────────────────────────────┤
│            Domain                   │ ← Entities, Value Objects, Services
├─────────────────────────────────────┤
│          Infrastructure             │ ← Repositories, External Services
└─────────────────────────────────────┘
```

## 🔒 Security Features

- **JWT Authentication** with secure secret keys
- **Password hashing** with bcrypt
- **Rate limiting** on authentication endpoints
- **CORS configuration** for cross-origin requests
- **Input validation** with FluentValidation
- **SQL injection protection** with EF Core
- **Security headers** middleware
- **HTTPS enforcement** in production

## 🚀 Performance

- **Entity Framework optimizations** with query tracking
- **Caching layer** with Redis (optional)
- **Async/await** throughout the application
- **Database indexing** on frequently queried columns
- **Connection pooling** for database connections
- **Compression** for API responses

## 🤝 Contributing

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Development Guidelines
- Follow **Clean Code** principles
- Write **unit tests** for new features
- Maintain **high code coverage** (>95%)
- Follow **conventional commits** format
- Update **documentation** as needed

## 📝 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Domain-Driven Design** concepts by Eric Evans
- **Clean Architecture** by Robert C. Martin
- **.NET Community** for excellent tools and libraries
- **Contributors** who help improve this project

## 📞 Support

- 📧 **Email**: support@volcanion-auth.com
- 🐛 **Issues**: [GitHub Issues](https://github.com/rickymta/volcanion-auth-ddd/issues)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/rickymta/volcanion-auth-ddd/discussions)
- 📖 **Documentation**: [Wiki](https://github.com/rickymta/volcanion-auth-ddd/wiki)

---

⭐ **Star this repository** if you find it helpful!

[![GitHub stars](https://img.shields.io/github/stars/rickymta/volcanion-auth-ddd.svg?style=social&label=Star)](https://github.com/rickymta/volcanion-auth-ddd)

