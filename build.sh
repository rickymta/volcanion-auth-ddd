#!/bin/bash

echo "🚀 Volcanion Auth DDD - Build & Run Script"
echo "=========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
check_docker() {
    print_status "Checking Docker..."
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker is not running. Please start Docker and try again."
        exit 1
    fi
    print_success "Docker is running"
}

# Start infrastructure services
start_infrastructure() {
    print_status "Starting infrastructure services..."
    docker-compose up -d
    
    if [ $? -eq 0 ]; then
        print_success "Infrastructure services started successfully"
        
        print_status "Waiting for services to be ready..."
        sleep 30
        
        # Check service health
        print_status "Checking service health..."
        
        # Check MySQL
        if docker exec volcanion-mysql mysqladmin ping -h localhost --silent; then
            print_success "MySQL is ready"
        else
            print_warning "MySQL might not be ready yet"
        fi
        
        # Check Redis
        if docker exec volcanion-redis redis-cli ping > /dev/null 2>&1; then
            print_success "Redis is ready"
        else
            print_warning "Redis might not be ready yet"
        fi
        
        # Check Elasticsearch
        if curl -s http://localhost:9200/_cluster/health > /dev/null 2>&1; then
            print_success "Elasticsearch is ready"
        else
            print_warning "Elasticsearch might not be ready yet"
        fi
        
    else
        print_error "Failed to start infrastructure services"
        exit 1
    fi
}

# Build the application
build_application() {
    print_status "Building the application..."
    dotnet restore
    
    if [ $? -eq 0 ]; then
        dotnet build --configuration Release
        if [ $? -eq 0 ]; then
            print_success "Application built successfully"
        else
            print_error "Failed to build application"
            exit 1
        fi
    else
        print_error "Failed to restore packages"
        exit 1
    fi
}

# Setup database
setup_database() {
    print_status "Setting up database..."
    
    cd src/Volcanion.Auth.Presentation
    
    # Check if migrations exist
    if [ ! -d "../Volcanion.Auth.Infrastructure/Migrations" ]; then
        print_status "Creating initial migration..."
        dotnet ef migrations add InitialCreate --project ../Volcanion.Auth.Infrastructure
    fi
    
    print_status "Updating database..."
    dotnet ef database update --project ../Volcanion.Auth.Infrastructure
    
    if [ $? -eq 0 ]; then
        print_success "Database setup completed"
    else
        print_error "Failed to setup database"
        exit 1
    fi
    
    cd ../..
}

# Run the application
run_application() {
    print_status "Starting the application..."
    cd src/Volcanion.Auth.Presentation
    
    print_success "🎉 Application is starting..."
    print_status "API will be available at:"
    print_status "  - Swagger UI: http://localhost:5000/swagger"
    print_status "  - HTTPS: https://localhost:5001"
    print_status "  - HTTP: http://localhost:5000"
    echo ""
    print_status "Other services:"
    print_status "  - Kibana: http://localhost:5601"
    print_status "  - Grafana: http://localhost:3000 (admin/admin123)"
    print_status "  - Prometheus: http://localhost:9090"
    echo ""
    print_warning "Press Ctrl+C to stop the application"
    echo ""
    
    dotnet run
}

# Stop infrastructure
stop_infrastructure() {
    print_status "Stopping infrastructure services..."
    docker-compose down
    print_success "Infrastructure services stopped"
}

# Show help
show_help() {
    echo "Usage: ./build.sh [OPTION]"
    echo ""
    echo "Options:"
    echo "  start, run       Start infrastructure and run the application"
    echo "  build           Build the application only"
    echo "  infra           Start infrastructure services only"
    echo "  db              Setup database only"
    echo "  stop            Stop infrastructure services"
    echo "  clean           Clean build artifacts and stop services"
    echo "  help            Show this help message"
    echo ""
    echo "Examples:"
    echo "  ./build.sh start       # Full startup (infrastructure + app)"
    echo "  ./build.sh infra       # Start infrastructure only"
    echo "  ./build.sh build       # Build application only"
    echo "  ./build.sh stop        # Stop all services"
}

# Clean everything
clean_all() {
    print_status "Cleaning up..."
    
    # Stop services
    stop_infrastructure
    
    # Clean build artifacts
    print_status "Cleaning build artifacts..."
    dotnet clean
    
    # Remove Docker volumes (optional)
    read -p "Do you want to remove Docker volumes (this will delete all data)? [y/N]: " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        print_status "Removing Docker volumes..."
        docker-compose down -v
        print_warning "All data has been removed"
    fi
    
    print_success "Cleanup completed"
}

# Main script logic
case "${1:-start}" in
    "start"|"run")
        check_docker
        start_infrastructure
        build_application
        setup_database
        run_application
        ;;
    "build")
        build_application
        ;;
    "infra")
        check_docker
        start_infrastructure
        ;;
    "db")
        setup_database
        ;;
    "stop")
        stop_infrastructure
        ;;
    "clean")
        clean_all
        ;;
    "help"|"-h"|"--help")
        show_help
        ;;
    *)
        print_error "Unknown option: $1"
        show_help
        exit 1
        ;;
esac
