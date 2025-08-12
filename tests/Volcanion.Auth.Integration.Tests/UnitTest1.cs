using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volcanion.Auth.Infrastructure.Data;
using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.ValueObjects;
using Volcanion.Auth.Infrastructure.Repositories;

namespace Volcanion.Auth.Integration.Tests;

public class DatabaseIntegrationTestBase : IAsyncLifetime
{
    protected AuthDbContext DbContext { get; private set; } = null!;
    private readonly ServiceProvider _serviceProvider;

    public DatabaseIntegrationTestBase()
    {
        var services = new ServiceCollection();
        
        // Use InMemory database instead of Docker MySQL for testing
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
        });
        services.AddLogging();

        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        DbContext = _serviceProvider.GetRequiredService<AuthDbContext>();
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _serviceProvider.DisposeAsync();
    }
}

public class DatabaseConnectionTests : DatabaseIntegrationTestBase
{
    [Fact]
    public async Task Database_Should_Be_Created_And_Accessible()
    {
        // Act
        var canConnect = await DbContext.Database.CanConnectAsync();

        // Assert
        Assert.True(canConnect);
    }

    [Fact]
    public async Task Database_Should_Have_Users_Table()
    {
        // Arrange & Act
        var user = new User("Test", "User", Email.Create("test@example.com"), Password.CreateFromPlainText("Password123!"));
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();

        // Assert - Check if we can query users table
        var userCount = await DbContext.Users.CountAsync();
        Assert.Equal(1, userCount);
    }
}

public class UserRepositoryIntegrationTests : DatabaseIntegrationTestBase
{
    [Fact]
    public async Task UserRepository_Should_Save_And_Retrieve_User()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var password = Password.CreateFromPlainText("Password123!");
        var user = new User("John", "Doe", email, password);
        
        var repository = new UserRepository(DbContext);

        // Act
        await repository.AddAsync(user);
        await DbContext.SaveChangesAsync();

        var retrievedUser = await repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal(user.Id, retrievedUser.Id);
        Assert.Equal(user.Email.Value, retrievedUser.Email.Value);
        Assert.Equal(user.FirstName, retrievedUser.FirstName);
        Assert.Equal(user.LastName, retrievedUser.LastName);
    }

    [Fact]
    public async Task UserRepository_Should_Find_User_By_Email()
    {
        // Arrange
        var email = Email.Create("findme@example.com");
        var password = Password.CreateFromPlainText("Password123!");
        var user = new User("Jane", "Smith", email, password);
        
        var repository = new UserRepository(DbContext);

        // Act
        await repository.AddAsync(user);
        await DbContext.SaveChangesAsync();

        var foundUser = await repository.GetByEmailAsync(email);

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(user.Id, foundUser.Id);
        Assert.Equal(email.Value, foundUser.Email.Value);
    }

    [Fact]
    public async Task UserRepository_Should_Update_User()
    {
        // Arrange
        var email = Email.Create("update@example.com");
        var password = Password.CreateFromPlainText("Password123!");
        var user = new User("Old", "Name", email, password);
        
        var repository = new UserRepository(DbContext);
        await repository.AddAsync(user);
        await DbContext.SaveChangesAsync();

        // Act
        user.UpdatePersonalInfo("New", "Name");
        await repository.UpdateAsync(user);
        await DbContext.SaveChangesAsync();

        var updatedUser = await repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(updatedUser);
        Assert.Equal("New", updatedUser.FirstName);
        Assert.Equal("Name", updatedUser.LastName);
    }

    [Fact]
    public async Task UserRepository_Should_Delete_User()
    {
        // Arrange
        var email = Email.Create("delete@example.com");
        var password = Password.CreateFromPlainText("Password123!");
        var user = new User("Delete", "Me", email, password);
        
        var repository = new UserRepository(DbContext);
        await repository.AddAsync(user);
        await DbContext.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(user);
        await DbContext.SaveChangesAsync();

        var deletedUser = await repository.GetByIdAsync(user.Id);

        // Assert
        Assert.Null(deletedUser);
    }
}

public class RoleRepositoryIntegrationTests : DatabaseIntegrationTestBase
{
    [Fact]
    public async Task RoleRepository_Should_Save_And_Retrieve_Role()
    {
        // Arrange
        var role = new Role("TestRole", "A test role");
        var repository = new RoleRepository(DbContext);

        // Act
        await repository.AddAsync(role);
        await DbContext.SaveChangesAsync();

        var retrievedRole = await repository.GetByIdAsync(role.Id);

        // Assert
        Assert.NotNull(retrievedRole);
        Assert.Equal(role.Id, retrievedRole.Id);
        Assert.Equal(role.Name, retrievedRole.Name);
        Assert.Equal(role.Description, retrievedRole.Description);
    }

    [Fact]
    public async Task RoleRepository_Should_Find_Role_By_Name()
    {
        // Arrange
        var role = new Role("UniqueRole", "A unique role");
        var repository = new RoleRepository(DbContext);

        // Act
        await repository.AddAsync(role);
        await DbContext.SaveChangesAsync();

        var foundRole = await repository.GetByNameAsync("UniqueRole");

        // Assert
        Assert.NotNull(foundRole);
        Assert.Equal(role.Id, foundRole.Id);
        Assert.Equal("UniqueRole", foundRole.Name);
    }
}
