namespace Volcanion.Auth.Infrastructure.Configuration;

public class AppSettings
{
    public string BaseUrl { get; set; } = "https://localhost:7001";
}

public class EmailSettings
{
    public string SmtpHost { get; set; } = "localhost";
    public int SmtpPort { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string FromEmail { get; set; } = "noreply@volcanion.com";
    public string FromName { get; set; } = "Volcanion Auth";
}

public class SmsSettings
{
    public string Provider { get; set; } = "Mock"; // Mock, Twilio, AWS
    public TwilioSettings TwilioSettings { get; set; } = new();
    public AwsSnsSettings AwsSnsSettings { get; set; } = new();
}

public class TwilioSettings
{
    public string? AccountSid { get; set; }
    public string? AuthToken { get; set; }
    public string? FromPhoneNumber { get; set; }
}

public class AwsSnsSettings
{
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string Region { get; set; } = "us-east-1";
}

public class FileStorageSettings
{
    public string StoragePath { get; set; } = "uploads";
    public long MaxFileSizeBytes { get; set; } = 5242880; // 5MB
    public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };
}

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; } = { "http://localhost:3000", "https://localhost:3000" };
    public string[] AllowedMethods { get; set; } = { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
    public string[] AllowedHeaders { get; set; } = { "Content-Type", "Authorization" };
}

public class SwaggerSettings
{
    public string Title { get; set; } = "Volcanion Auth API";
    public string Version { get; set; } = "v1";
    public string Description { get; set; } = "Authentication and Authorization API using DDD Architecture";
}
