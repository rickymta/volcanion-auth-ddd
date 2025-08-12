using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Volcanion.Auth.Infrastructure.Configuration;

namespace Volcanion.Auth.Infrastructure.External;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    Task SendVerificationEmailAsync(string to, string verificationToken);
    Task SendPasswordResetEmailAsync(string to, string resetToken);
    Task SendWelcomeEmailAsync(string to, string firstName);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;
    private readonly string _baseUrl;

    public EmailService(IOptions<EmailSettings> emailSettings, IConfiguration configuration, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
        _baseUrl = configuration["AppSettings:BaseUrl"] ?? "https://localhost:7001";
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
                bodyBuilder.HtmlBody = body;
            else
                bodyBuilder.TextBody = body;

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, 
                _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            
            if (!string.IsNullOrEmpty(_emailSettings.Username) && !string.IsNullOrEmpty(_emailSettings.Password))
            {
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendVerificationEmailAsync(string to, string verificationToken)
    {
        var subject = "Email Verification - Volcanion Auth";
        var body = $@"
            <h2>Email Verification</h2>
            <p>Please click the link below to verify your email address:</p>
            <a href='{_baseUrl}/api/auth/verify-email?token={verificationToken}'>Verify Email</a>
            <p>This link will expire in 24 hours.</p>
            <p>If you didn't create an account, please ignore this email.</p>";
        
        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken)
    {
        var subject = "Password Reset - Volcanion Auth";
        var body = $@"
            <h2>Password Reset</h2>
            <p>You requested a password reset. Click the link below to reset your password:</p>
            <a href='{_baseUrl}/reset-password?token={resetToken}'>Reset Password</a>
            <p>This link will expire in 1 hour.</p>
            <p>If you didn't request this, please ignore this email.</p>";
        
        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendWelcomeEmailAsync(string to, string firstName)
    {
        var subject = "Welcome to Volcanion Auth";
        var body = $@"
            <h2>Welcome {firstName}!</h2>
            <p>Your account has been created successfully.</p>
            <p>You can now log in and start using our services.</p>
            <p>If you have any questions, please contact our support team.</p>
            <p>Thank you for joining us!</p>";
        
        await SendEmailAsync(to, subject, body, true);
    }
}
