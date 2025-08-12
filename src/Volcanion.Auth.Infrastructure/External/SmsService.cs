using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Volcanion.Auth.Infrastructure.Configuration;

namespace Volcanion.Auth.Infrastructure.External;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message);
    Task SendVerificationCodeAsync(string phoneNumber, string code);
    Task SendLoginAlertAsync(string phoneNumber, string deviceInfo);
}

public class SmsService : ISmsService
{
    private readonly SmsSettings _smsSettings;
    private readonly ILogger<SmsService> _logger;
    private readonly HttpClient _httpClient;

    public SmsService(IOptions<SmsSettings> smsSettings, ILogger<SmsService> logger, HttpClient httpClient)
    {
        _smsSettings = smsSettings.Value;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            var provider = _smsSettings.Provider?.ToLower() ?? "mock";

            switch (provider)
            {
                case "twilio":
                    await SendViaTwilioAsync(phoneNumber, message);
                    break;
                case "aws":
                    await SendViaAwsSnsAsync(phoneNumber, message);
                    break;
                default:
                    await SendViaMockAsync(phoneNumber, message);
                    break;
            }

            _logger.LogInformation("SMS sent successfully to {PhoneNumber}", phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", phoneNumber);
            throw;
        }
    }

    public async Task SendVerificationCodeAsync(string phoneNumber, string code)
    {
        var message = $"Your verification code is: {code}. Valid for 5 minutes. Do not share this code with anyone.";
        await SendSmsAsync(phoneNumber, message);
    }

    public async Task SendLoginAlertAsync(string phoneNumber, string deviceInfo)
    {
        var message = $"New login detected from {deviceInfo} at {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC. If this wasn't you, please secure your account immediately.";
        await SendSmsAsync(phoneNumber, message);
    }

    private async Task SendViaTwilioAsync(string phoneNumber, string message)
    {
        var twilioSettings = _smsSettings.TwilioSettings;

        if (string.IsNullOrEmpty(twilioSettings.AccountSid) || string.IsNullOrEmpty(twilioSettings.AuthToken))
        {
            throw new InvalidOperationException("Twilio credentials not configured");
        }

        var url = $"https://api.twilio.com/2010-04-01/Accounts/{twilioSettings.AccountSid}/Messages.json";
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{twilioSettings.AccountSid}:{twilioSettings.AuthToken}"));

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("To", phoneNumber),
            new KeyValuePair<string, string>("From", twilioSettings.FromPhoneNumber ?? ""),
            new KeyValuePair<string, string>("Body", message)
        });

        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
    }

    private async Task SendViaAwsSnsAsync(string phoneNumber, string message)
    {
        // Placeholder for AWS SNS implementation
        // Would require AWS SDK for .NET
        _logger.LogInformation("AWS SNS SMS sending not implemented. Message: {Message}", message);
        await Task.CompletedTask;
    }

    private async Task SendViaMockAsync(string phoneNumber, string message)
    {
        // Mock implementation for development/testing
        _logger.LogInformation("MOCK SMS sent to {PhoneNumber}: {Message}", phoneNumber, message);
        
        // Simulate network delay
        await Task.Delay(500);
        
        // In a real mock service, you might save to a file or database for testing verification
        var mockLogPath = Path.Combine(Directory.GetCurrentDirectory(), "mock_sms.log");
        var logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {phoneNumber} | {message}{Environment.NewLine}";
        await File.AppendAllTextAsync(mockLogPath, logEntry);
    }
}
