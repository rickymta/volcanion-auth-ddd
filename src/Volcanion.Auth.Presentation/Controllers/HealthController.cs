using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Volcanion.Auth.Infrastructure.Data;

namespace Volcanion.Auth.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AuthDbContext _dbContext;
    private readonly IConnectionMultiplexer _redis;

    public HealthController(AuthDbContext dbContext, IConnectionMultiplexer redis)
    {
        _dbContext = dbContext;
        _redis = redis;
    }

    /// <summary>
    /// Basic health check
    /// </summary>
    /// <returns>Application health status</returns>
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Detailed health check including database and Redis
    /// </summary>
    /// <returns>Detailed health status</returns>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailedHealth()
    {
        var dbHealth = await CheckDatabaseHealth();
        var redisHealth = await CheckRedisHealth();
        
        var healthStatus = new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            checks = new
            {
                database = dbHealth,
                redis = redisHealth
            }
        };

        var isHealthy = ((dynamic)dbHealth).healthy && ((dynamic)redisHealth).healthy;
        
        if (isHealthy)
        {
            return Ok(healthStatus);
        }
        
        return StatusCode(503, healthStatus);
    }

    private async Task<object> CheckDatabaseHealth()
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
            return new { healthy = true, responseTime = "< 100ms" };
        }
        catch (Exception ex)
        {
            return new { healthy = false, error = ex.Message };
        }
    }

    private async Task<object> CheckRedisHealth()
    {
        try
        {
            var database = _redis.GetDatabase();
            await database.PingAsync();
            return new { healthy = true, responseTime = "< 50ms" };
        }
        catch (Exception ex)
        {
            return new { healthy = false, error = ex.Message };
        }
    }
}
