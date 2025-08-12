# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Volcanion.Auth.Presentation/Volcanion.Auth.Presentation.csproj", "src/Volcanion.Auth.Presentation/"]
COPY ["src/Volcanion.Auth.Application/Volcanion.Auth.Application.csproj", "src/Volcanion.Auth.Application/"]
COPY ["src/Volcanion.Auth.Domain/Volcanion.Auth.Domain.csproj", "src/Volcanion.Auth.Domain/"]
COPY ["src/Volcanion.Auth.Infrastructure/Volcanion.Auth.Infrastructure.csproj", "src/Volcanion.Auth.Infrastructure/"]

RUN dotnet restore "src/Volcanion.Auth.Presentation/Volcanion.Auth.Presentation.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Volcanion.Auth.Presentation"
RUN dotnet build "Volcanion.Auth.Presentation.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Volcanion.Auth.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create uploads directory and non-root user
RUN mkdir -p /app/uploads && \
    adduser --disabled-password --home /app --gecos '' appuser && \
    chown -R appuser:appuser /app

USER appuser

COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Volcanion.Auth.Presentation.dll"]
