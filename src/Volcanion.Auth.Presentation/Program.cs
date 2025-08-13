using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Volcanion.Auth.Application.Common;
using Volcanion.Auth.Application.Interfaces;
using Volcanion.Auth.Application.Services;
using Volcanion.Auth.Domain.Repositories;
using Volcanion.Auth.Domain.Services;
using Volcanion.Auth.Infrastructure.Data;
using Volcanion.Auth.Infrastructure.External;
using Volcanion.Auth.Infrastructure.Logging;
using Volcanion.Auth.Infrastructure.Repositories;
using Volcanion.Auth.Infrastructure.Security;
using Volcanion.Auth.Infrastructure.Services;
using Volcanion.Auth.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<MappingProfile>());

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add Entity Framework
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var serverVersion = new MySqlServerVersion(new Version(8, 0, 43));
    options.UseMySql(connectionString, serverVersion);
});

// Add Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("RedisConnection");
    return ConnectionMultiplexer.Connect(connectionString ?? "localhost:6379");
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running"));

// Add JWT Authentication
var jwtSecret = builder.Configuration["JwtSettings:Key"];
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret ?? ""))
    };
});

builder.Services.AddAuthorization();

// Add CORS
builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("CorsSettings"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>() ?? new CorsSettings();
        policy.WithOrigins(corsSettings.AllowedOrigins)
              .WithMethods(corsSettings.AllowedMethods)
              .WithHeaders(corsSettings.AllowedHeaders)
              .AllowCredentials();
    });
});

// Register repositories and services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();

// Register domain services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

// Register application services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICacheService, CacheService>();

// Register database seeder
// builder.Services.AddHostedService<DatabaseSeederService>();

// Register external services
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));
builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection("FileStorageSettings"));
builder.Services.AddHttpClient(); // For SmsService
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

// Register security services
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IRateLimitingService, RateLimitingService>();

// Register logging services
builder.Services.Configure<ElasticsearchSettings>(builder.Configuration.GetSection("ElasticsearchSettings"));
builder.Services.AddScoped<IElasticsearchLogger, ElasticsearchLogger>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();

// Register business services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

// Add Swagger/OpenAPI
builder.Services.Configure<SwaggerSettings>(builder.Configuration.GetSection("SwaggerSettings"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var swaggerSettings = builder.Configuration.GetSection("SwaggerSettings").Get<SwaggerSettings>() ?? new SwaggerSettings();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = swaggerSettings.Title,
        Version = swaggerSettings.Version,
        Description = swaggerSettings.Description
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Volcanion Auth API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
