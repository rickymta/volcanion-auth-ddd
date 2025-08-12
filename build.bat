@echo off
setlocal EnableDelayedExpansion

echo 🚀 Volcanion Auth DDD - Build ^& Run Script
echo ==========================================

REM Function to print colored output (simplified for Windows)
set "INFO=[INFO]"
set "SUCCESS=[SUCCESS]"
set "WARNING=[WARNING]"
set "ERROR=[ERROR]"

REM Check if Docker is running
:check_docker
echo %INFO% Checking Docker...
docker info >nul 2>&1
if errorlevel 1 (
    echo %ERROR% Docker is not running. Please start Docker and try again.
    exit /b 1
)
echo %SUCCESS% Docker is running

REM Start infrastructure services
:start_infrastructure
echo %INFO% Starting infrastructure services...
docker-compose up -d
if errorlevel 1 (
    echo %ERROR% Failed to start infrastructure services
    exit /b 1
)
echo %SUCCESS% Infrastructure services started successfully

echo %INFO% Waiting for services to be ready...
timeout /t 30 /nobreak >nul

echo %INFO% Checking service health...

REM Check MySQL
docker exec volcanion-mysql mysqladmin ping -h localhost --silent >nul 2>&1
if not errorlevel 1 (
    echo %SUCCESS% MySQL is ready
) else (
    echo %WARNING% MySQL might not be ready yet
)

REM Check Redis
docker exec volcanion-redis redis-cli ping >nul 2>&1
if not errorlevel 1 (
    echo %SUCCESS% Redis is ready
) else (
    echo %WARNING% Redis might not be ready yet
)

goto :build_application

REM Build the application
:build_application
echo %INFO% Building the application...
dotnet restore
if errorlevel 1 (
    echo %ERROR% Failed to restore packages
    exit /b 1
)

dotnet build --configuration Release
if errorlevel 1 (
    echo %ERROR% Failed to build application
    exit /b 1
)
echo %SUCCESS% Application built successfully
goto :setup_database

REM Setup database
:setup_database
echo %INFO% Setting up database...
cd src\Volcanion.Auth.Presentation

REM Check if migrations exist
if not exist "..\Volcanion.Auth.Infrastructure\Migrations" (
    echo %INFO% Creating initial migration...
    dotnet ef migrations add InitialCreate --project ..\Volcanion.Auth.Infrastructure
)

echo %INFO% Updating database...
dotnet ef database update --project ..\Volcanion.Auth.Infrastructure
if errorlevel 1 (
    echo %ERROR% Failed to setup database
    cd ..\..
    exit /b 1
)
echo %SUCCESS% Database setup completed
cd ..\..
goto :run_application

REM Run the application
:run_application
echo %INFO% Starting the application...
cd src\Volcanion.Auth.Presentation

echo %SUCCESS% 🎉 Application is starting...
echo %INFO% API will be available at:
echo %INFO%   - Swagger UI: http://localhost:5000/swagger
echo %INFO%   - HTTPS: https://localhost:5001
echo %INFO%   - HTTP: http://localhost:5000
echo.
echo %INFO% Other services:
echo %INFO%   - Kibana: http://localhost:5601
echo %INFO%   - Grafana: http://localhost:3000 (admin/admin123)
echo %INFO%   - Prometheus: http://localhost:9090
echo.
echo %WARNING% Press Ctrl+C to stop the application
echo.

dotnet run
goto :eof

REM Stop infrastructure
:stop_infrastructure
echo %INFO% Stopping infrastructure services...
docker-compose down
echo %SUCCESS% Infrastructure services stopped
goto :eof

REM Show help
:show_help
echo Usage: build.bat [OPTION]
echo.
echo Options:
echo   start, run       Start infrastructure and run the application
echo   build           Build the application only
echo   infra           Start infrastructure services only
echo   db              Setup database only
echo   stop            Stop infrastructure services
echo   clean           Clean build artifacts and stop services
echo   help            Show this help message
echo.
echo Examples:
echo   build.bat start       # Full startup (infrastructure + app)
echo   build.bat infra       # Start infrastructure only
echo   build.bat build       # Build application only
echo   build.bat stop        # Stop all services
goto :eof

REM Clean everything
:clean_all
echo %INFO% Cleaning up...

call :stop_infrastructure

echo %INFO% Cleaning build artifacts...
dotnet clean

set /p "choice=Do you want to remove Docker volumes (this will delete all data)? [y/N]: "
if /i "!choice!"=="y" (
    echo %INFO% Removing Docker volumes...
    docker-compose down -v
    echo %WARNING% All data has been removed
)

echo %SUCCESS% Cleanup completed
goto :eof

REM Main script logic
set "command=%~1"
if "%command%"=="" set "command=start"

if "%command%"=="start" goto :main_start
if "%command%"=="run" goto :main_start
if "%command%"=="build" goto :build_application
if "%command%"=="infra" goto :main_infra
if "%command%"=="db" goto :setup_database
if "%command%"=="stop" goto :stop_infrastructure
if "%command%"=="clean" goto :clean_all
if "%command%"=="help" goto :show_help
if "%command%"=="-h" goto :show_help
if "%command%"=="--help" goto :show_help

echo %ERROR% Unknown option: %command%
call :show_help
exit /b 1

:main_start
call :check_docker
call :start_infrastructure
call :build_application
call :setup_database
call :run_application
goto :eof

:main_infra
call :check_docker
call :start_infrastructure
goto :eof
