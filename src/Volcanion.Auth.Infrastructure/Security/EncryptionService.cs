using System.Security.Cryptography;
using System.Text;

namespace Volcanion.Auth.Infrastructure.Security;

public interface IEncryptionService
{
    string Encrypt(string plainText, string key);
    string Decrypt(string cipherText, string key);
    string GenerateSecureToken(int length = 32);
    string HashData(string data);
    bool VerifyHash(string data, string hash);
}

public class EncryptionService : IEncryptionService
{
    public string Encrypt(string plainText, string key)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32, '0')[..32]);
        aes.IV = new byte[16]; // Simple IV for demo - use random IV in production

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        
        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string cipherText, string key)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32, '0')[..32]);
        aes.IV = new byte[16]; // Simple IV for demo - use random IV in production

        using var decryptor = aes.CreateDecryptor();
        var cipherBytes = Convert.FromBase64String(cipherText);
        var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public string GenerateSecureToken(int length = 32)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public string HashData(string data)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyHash(string data, string hash)
    {
        var dataHash = HashData(data);
        return dataHash == hash;
    }
}
