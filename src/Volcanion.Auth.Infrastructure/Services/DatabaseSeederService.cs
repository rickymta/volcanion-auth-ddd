using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Infrastructure.Data;

namespace Volcanion.Auth.Infrastructure.Services;

public class DatabaseSeederService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeederService> _logger;

    public DatabaseSeederService(IServiceProvider serviceProvider, ILogger<DatabaseSeederService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        try
        {
            // Ensure database is created
            await context.Database.MigrateAsync(cancellationToken);

            // Seed initial data
            await SeedPermissionsAsync(context, cancellationToken);
            await SeedRolesAsync(context, cancellationToken);
            await SeedAdminUserAsync(context, cancellationToken);

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task SeedPermissionsAsync(AuthDbContext context, CancellationToken cancellationToken)
    {
        if (await context.Permissions.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Permissions already exist, skipping seed");
            return;
        }

        var permissions = new List<Domain.Entities.Permission>();

        // User permissions
        permissions.Add(new Domain.Entities.Permission("users.read", "Read Users", "User", "Read"));
        permissions.Add(new Domain.Entities.Permission("users.write", "Write Users", "User", "Write"));
        permissions.Add(new Domain.Entities.Permission("users.delete", "Delete Users", "User", "Delete"));
        permissions.Add(new Domain.Entities.Permission("users.manage", "Manage Users", "User", "Manage"));

        // Role permissions
        permissions.Add(new Domain.Entities.Permission("roles.read", "Read Roles", "Role", "Read"));
        permissions.Add(new Domain.Entities.Permission("roles.write", "Write Roles", "Role", "Write"));
        permissions.Add(new Domain.Entities.Permission("roles.delete", "Delete Roles", "Role", "Delete"));
        permissions.Add(new Domain.Entities.Permission("roles.manage", "Manage Roles", "Role", "Manage"));

        // Permission permissions
        permissions.Add(new Domain.Entities.Permission("permissions.read", "Read Permissions", "Permission", "Read"));
        permissions.Add(new Domain.Entities.Permission("permissions.write", "Write Permissions", "Permission", "Write"));
        permissions.Add(new Domain.Entities.Permission("permissions.delete", "Delete Permissions", "Permission", "Delete"));
        permissions.Add(new Domain.Entities.Permission("permissions.manage", "Manage Permissions", "Permission", "Manage"));

        // System permissions
        permissions.Add(new Domain.Entities.Permission("system.admin", "System Administration", "System", "Admin"));
        permissions.Add(new Domain.Entities.Permission("system.audit", "System Audit", "System", "Audit"));
        permissions.Add(new Domain.Entities.Permission("system.backup", "System Backup", "System", "Backup"));

        // Profile permissions
        permissions.Add(new Domain.Entities.Permission("profile.read", "Read Own Profile", "Profile", "Read"));
        permissions.Add(new Domain.Entities.Permission("profile.write", "Update Own Profile", "Profile", "Write"));
        permissions.Add(new Domain.Entities.Permission("profile.password", "Change Own Password", "Profile", "Password"));

        // Auth permissions
        permissions.Add(new Domain.Entities.Permission("auth.login", "Login", "Auth", "Login"));
        permissions.Add(new Domain.Entities.Permission("auth.logout", "Logout", "Auth", "Logout"));
        permissions.Add(new Domain.Entities.Permission("auth.register", "Register", "Auth", "Register"));
        permissions.Add(new Domain.Entities.Permission("auth.refresh", "Refresh Token", "Auth", "Refresh"));

        context.Permissions.AddRange(permissions);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Seeded {permissions.Count} permissions");
    }

    private async Task SeedRolesAsync(AuthDbContext context, CancellationToken cancellationToken)
    {
        if (await context.Roles.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Roles already exist, skipping seed");
            return;
        }

        // Get all permissions
        var allPermissions = await context.Permissions.ToListAsync(cancellationToken);
        
        // Create Admin role with all permissions
        var adminRole = new Domain.Entities.Role("Admin", "System Administrator with full access");
        // TODO: Implement permission assignment through RolePermission entity

        // Create User role with basic permissions
        var userRole = new Domain.Entities.Role("User", "Standard user with basic access");
        // TODO: Implement permission assignment through RolePermission entity

        // Create Manager role with user management permissions
        var managerRole = new Domain.Entities.Role("Manager", "Manager with user oversight capabilities");
        // TODO: Implement permission assignment through RolePermission entity

        var roles = new List<Domain.Entities.Role> { adminRole, userRole, managerRole };
        context.Roles.AddRange(roles);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Seeded {roles.Count} roles");
    }

    private async Task SeedAdminUserAsync(AuthDbContext context, CancellationToken cancellationToken)
    {
        var adminEmailString = "admin@volcanion.com";
        
        if (await context.Users.AnyAsync(u => u.Email.Value == adminEmailString, cancellationToken))
        {
            _logger.LogInformation("Admin user already exists, skipping seed");
            return;
        }

        // Get admin role
        var adminRole = await context.Roles
            .FirstOrDefaultAsync(r => r.Name == "Admin", cancellationToken);

        if (adminRole == null)
        {
            _logger.LogWarning("Admin role not found, cannot create admin user");
            return;
        }

        // Create admin user (you should change this password in production)
        try
        {
            var adminEmail = Domain.ValueObjects.Email.Create(adminEmailString);
            var adminPasswordResult = Domain.ValueObjects.Password.Create("Admin123!");
            
            if (adminPasswordResult.IsFailure)
            {
                _logger.LogError("Failed to create admin password: {Error}", adminPasswordResult.Error);
                return;
            }

            var adminUser = Domain.Entities.User.Create(
                email: adminEmail,
                password: adminPasswordResult.Value,
                firstName: "System",
                lastName: "Administrator"
            );

            // TODO: Implement role assignment through UserRole entity
            // adminUser.AssignRoles(new List<Role> { adminRole });
            adminUser.Activate(); // Activate the admin user

            context.Users.Add(adminUser);
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Seeded admin user with email: {Email}", adminEmailString);
            _logger.LogWarning("IMPORTANT: Change the default admin password immediately!");
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("Failed to create admin user: {Error}", ex.Message);
        }
    }
}
