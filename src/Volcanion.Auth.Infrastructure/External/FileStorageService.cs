using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volcanion.Auth.Infrastructure.Configuration;

namespace Volcanion.Auth.Infrastructure.External;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<string> UploadAvatarAsync(Stream imageStream, string userId, string fileExtension);
    Task<bool> DeleteFileAsync(string fileUrl);
    Task<Stream> DownloadFileAsync(string fileUrl);
}

public class FileStorageService : IFileStorageService
{
    private readonly FileStorageSettings _fileStorageSettings;
    private readonly ILogger<FileStorageService> _logger;
    private readonly string _storagePath;
    private readonly string _baseUrl;

    public FileStorageService(IOptions<FileStorageSettings> fileStorageSettings, IConfiguration configuration, ILogger<FileStorageService> logger)
    {
        _fileStorageSettings = fileStorageSettings.Value;
        _logger = logger;
        _baseUrl = configuration["AppSettings:BaseUrl"] ?? "https://localhost:7001";
        _storagePath = Path.IsPathRooted(_fileStorageSettings.StoragePath) 
            ? _fileStorageSettings.StoragePath 
            : Path.Combine(Directory.GetCurrentDirectory(), _fileStorageSettings.StoragePath);
        
        // Ensure storage directory exists
        Directory.CreateDirectory(_storagePath);
        Directory.CreateDirectory(Path.Combine(_storagePath, "avatars"));
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            // Generate unique filename to avoid conflicts
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_storagePath, uniqueFileName);

            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }

            var fileUrl = $"{_baseUrl}/uploads/{uniqueFileName}";

            _logger.LogInformation("File uploaded successfully: {FileName} -> {FileUrl}", fileName, fileUrl);
            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<string> UploadAvatarAsync(Stream imageStream, string userId, string fileExtension)
    {
        try
        {
            var fileName = $"avatar_{userId}{fileExtension}";
            var avatarDirectory = Path.Combine(_storagePath, "avatars");
            var filePath = Path.Combine(avatarDirectory, fileName);

            // Delete existing avatar if exists
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
            {
                await imageStream.CopyToAsync(fileStreamOutput);
            }

            var avatarUrl = $"{_baseUrl}/uploads/avatars/{fileName}";

            _logger.LogInformation("Avatar uploaded successfully for user: {UserId}", userId);
            return avatarUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload avatar for user: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            // Extract filename from URL
            var uri = new Uri(fileUrl);
            var fileName = Path.GetFileName(uri.LocalPath);
            var filePath = Path.Combine(_storagePath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted successfully: {FileUrl}", fileUrl);
                await Task.CompletedTask;
                return true;
            }

            _logger.LogWarning("File not found for deletion: {FileUrl}", fileUrl);
            await Task.CompletedTask;
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {FileUrl}", fileUrl);
            return false;
        }
    }

    public async Task<Stream> DownloadFileAsync(string fileUrl)
    {
        try
        {
            // Extract filename from URL
            var uri = new Uri(fileUrl);
            var fileName = Path.GetFileName(uri.LocalPath);
            var filePath = Path.Combine(_storagePath, fileName);

            if (File.Exists(filePath))
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                _logger.LogInformation("File downloaded successfully: {FileUrl}", fileUrl);
                await Task.CompletedTask;
                return fileStream;
            }

            throw new FileNotFoundException($"File not found: {fileUrl}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file: {FileUrl}", fileUrl);
            throw;
        }
    }

    private static string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
    }
}
