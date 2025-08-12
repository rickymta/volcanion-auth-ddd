using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Volcanion.Auth.Application.Interfaces;

namespace Volcanion.Auth.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly string _keyPrefix;

    public CacheService(IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _database = redis.GetDatabase();
        _keyPrefix = configuration["RedisSettings:KeyPrefix"] ?? "volcanion:auth:";
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetFullKey(key);
        var value = await _database.StringGetAsync(fullKey);
        
        if (!value.HasValue)
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(value.ToString());
        }
        catch
        {
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var fullKey = GetFullKey(key);
        var serializedValue = JsonSerializer.Serialize(value);
        
        await _database.StringSetAsync(fullKey, serializedValue, expiration);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetFullKey(key);
        await _database.KeyDeleteAsync(fullKey);
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
        var fullPattern = GetFullKey(pattern);
        
        var keys = server.Keys(pattern: fullPattern);
        var keyArray = keys.ToArray();
        
        if (keyArray.Any())
        {
            await _database.KeyDeleteAsync(keyArray);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var fullKey = GetFullKey(key);
        return await _database.KeyExistsAsync(fullKey);
    }

    private string GetFullKey(string key)
    {
        return $"{_keyPrefix}{key}";
    }
}
