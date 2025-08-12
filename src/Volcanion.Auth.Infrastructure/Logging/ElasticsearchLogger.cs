using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Text.Json;

namespace Volcanion.Auth.Infrastructure.Logging;

public class ElasticsearchSettings
{
    public string ConnectionString { get; set; } = "http://localhost:9200";
    public string IndexPrefix { get; set; } = "volcanion-auth";
    public bool Enabled { get; set; } = true;
}

public interface IElasticsearchLogger
{
    Task LogAsync(LogLevel level, string message, object? data = null, Exception? exception = null);
    Task LogUserActionAsync(string userId, string action, object? metadata = null);
    Task LogSecurityEventAsync(string eventType, string description, object? details = null);
}

public class ElasticsearchLogger : IElasticsearchLogger, IDisposable
{
    private readonly ILogger<ElasticsearchLogger> _logger;
    private readonly ElasticsearchSettings _settings;
    private readonly Serilog.ILogger _elasticLogger;
    private bool _disposed = false;

    public ElasticsearchLogger(
        ILogger<ElasticsearchLogger> logger,
        IOptions<ElasticsearchSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;

        // Configure Serilog with Elasticsearch sink
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithProperty("Application", "VolcanionAuth")
            .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development");

        if (_settings.Enabled && !string.IsNullOrEmpty(_settings.ConnectionString))
        {
            try
            {
                var elasticsearchSinkOptions = new ElasticsearchSinkOptions(new Uri(_settings.ConnectionString))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = $"{_settings.IndexPrefix}-logs-{{0:yyyy.MM.dd}}",
                    NumberOfShards = 2,
                    NumberOfReplicas = 1,
                    BatchPostingLimit = 50,
                    Period = TimeSpan.FromSeconds(5),
                    InlineFields = true,
                    MinimumLogEventLevel = Serilog.Events.LogEventLevel.Debug
                };

                loggerConfig.WriteTo.Elasticsearch(elasticsearchSinkOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to configure Elasticsearch sink, falling back to console logging");
            }
        }

        // Always add console sink as fallback
        loggerConfig.WriteTo.Console();

        _elasticLogger = loggerConfig.CreateLogger();
    }

    public async Task LogAsync(LogLevel level, string message, object? data = null, Exception? exception = null)
    {
        var logEntry = new
        {
            Timestamp = DateTime.UtcNow,
            Level = level.ToString(),
            Message = message,
            Data = data,
            Exception = exception?.ToString(),
            Application = "VolcanionAuth",
            TraceId = System.Diagnostics.Activity.Current?.Id
        };

        try
        {
            // Use Serilog for Elasticsearch logging
            var template = "Application Log: {Message} {@LogData}";
            
            switch (level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    _elasticLogger.Debug(template, message, logEntry);
                    break;
                case LogLevel.Information:
                    _elasticLogger.Information(template, message, logEntry);
                    break;
                case LogLevel.Warning:
                    _elasticLogger.Warning(template, message, logEntry);
                    break;
                case LogLevel.Error:
                    if (exception != null)
                        _elasticLogger.Error(exception, template, message, logEntry);
                    else
                        _elasticLogger.Error(template, message, logEntry);
                    break;
                case LogLevel.Critical:
                    if (exception != null)
                        _elasticLogger.Fatal(exception, template, message, logEntry);
                    else
                        _elasticLogger.Fatal(template, message, logEntry);
                    break;
            }
        }
        catch (Exception ex)
        {
            // Fallback to standard logger if Elasticsearch fails
            _logger.LogError(ex, "Failed to log to Elasticsearch: {Message}", message);
            _logger.Log(level, "{LogEntry}", JsonSerializer.Serialize(logEntry));
        }

        await Task.CompletedTask;
    }

    public async Task LogUserActionAsync(string userId, string action, object? metadata = null)
    {
        var actionLog = new
        {
            UserId = userId,
            Action = action,
            Metadata = metadata,
            Timestamp = DateTime.UtcNow,
            Type = "UserAction"
        };

        await LogAsync(LogLevel.Information, $"User action: {action}", actionLog);
    }

    public async Task LogSecurityEventAsync(string eventType, string description, object? details = null)
    {
        var securityLog = new
        {
            EventType = eventType,
            Description = description,
            Details = details,
            Timestamp = DateTime.UtcNow,
            Type = "SecurityEvent",
            TraceId = System.Diagnostics.Activity.Current?.Id,
            IpAddress = GetClientIpAddress()
        };

        await LogAsync(LogLevel.Warning, $"Security event: {eventType}", securityLog);
    }

    private string? GetClientIpAddress()
    {
        try
        {
            // In a real application, this would get the client IP from HttpContext
            // For now, return localhost as placeholder
            return "127.0.0.1";
        }
        catch
        {
            return null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            if (_elasticLogger is IDisposable disposableLogger)
            {
                disposableLogger.Dispose();
            }
            _disposed = true;
        }
    }
}
