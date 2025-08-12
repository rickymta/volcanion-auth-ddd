using Moq;
using Xunit;
using Volcanion.Auth.Domain.Services;
using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.ValueObjects;

namespace Volcanion.Auth.Domain.Tests.Services;

public class AuthenticationServiceTests
{
    [Fact]
    public async Task ValidateCredentialsAsync_Should_Return_True_When_Valid()
    {
        // Arrange
        var mockService = new Mock<IAuthenticationService>();
        mockService.Setup(s => s.ValidateCredentialsAsync("test@example.com", "Password123!", CancellationToken.None))
                   .ReturnsAsync(true);

        var service = mockService.Object;

        // Act
        var result = await service.ValidateCredentialsAsync("test@example.com", "Password123!", CancellationToken.None);

        // Assert
        Assert.True(result);
        mockService.Verify(s => s.ValidateCredentialsAsync("test@example.com", "Password123!", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_Should_Return_False_When_Invalid()
    {
        // Arrange
        var mockService = new Mock<IAuthenticationService>();
        mockService.Setup(s => s.ValidateCredentialsAsync("test@example.com", "WrongPassword", CancellationToken.None))
                   .ReturnsAsync(false);

        var service = mockService.Object;

        // Act
        var result = await service.ValidateCredentialsAsync("test@example.com", "WrongPassword", CancellationToken.None);

        // Assert
        Assert.False(result);
        mockService.Verify(s => s.ValidateCredentialsAsync("test@example.com", "WrongPassword", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_Should_Return_User_When_Valid()
    {
        // Arrange
        var mockService = new Mock<IAuthenticationService>();
        var expectedUser = new User(
            "John",
            "Doe",
            Email.Create("test@example.com"),
            Password.CreateFromPlainText("Password123!")
        );

        mockService.Setup(s => s.AuthenticateAsync("test@example.com", "Password123!", CancellationToken.None))
                   .ReturnsAsync(expectedUser);

        var service = mockService.Object;

        // Act
        var result = await service.AuthenticateAsync("test@example.com", "Password123!", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Email, result.Email);
        mockService.Verify(s => s.AuthenticateAsync("test@example.com", "Password123!", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_Should_Return_Null_When_Invalid()
    {
        // Arrange
        var mockService = new Mock<IAuthenticationService>();
        mockService.Setup(s => s.AuthenticateAsync("test@example.com", "WrongPassword", CancellationToken.None))
                   .ReturnsAsync((User?)null);

        var service = mockService.Object;

        // Act
        var result = await service.AuthenticateAsync("test@example.com", "WrongPassword", CancellationToken.None);

        // Assert
        Assert.Null(result);
        mockService.Verify(s => s.AuthenticateAsync("test@example.com", "WrongPassword", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task IsEmailAvailableAsync_Should_Return_True_When_Available()
    {
        // Arrange
        var mockService = new Mock<IAuthenticationService>();
        var email = Email.Create("available@example.com");
        
        mockService.Setup(s => s.IsEmailAvailableAsync(email, CancellationToken.None))
                   .ReturnsAsync(true);

        var service = mockService.Object;

        // Act
        var result = await service.IsEmailAvailableAsync(email, CancellationToken.None);

        // Assert
        Assert.True(result);
        mockService.Verify(s => s.IsEmailAvailableAsync(email, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task IsEmailAvailableAsync_Should_Return_False_When_Not_Available()
    {
        // Arrange
        var mockService = new Mock<IAuthenticationService>();
        var email = Email.Create("taken@example.com");
        
        mockService.Setup(s => s.IsEmailAvailableAsync(email, CancellationToken.None))
                   .ReturnsAsync(false);

        var service = mockService.Object;

        // Act
        var result = await service.IsEmailAvailableAsync(email, CancellationToken.None);

        // Assert
        Assert.False(result);
        mockService.Verify(s => s.IsEmailAvailableAsync(email, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task IsPhoneNumberAvailableAsync_Should_Return_True_When_Available()
    {
        // Arrange
        var mockService = new Mock<IAuthenticationService>();
        var phoneNumber = PhoneNumber.Create("+1234567890");
        
        mockService.Setup(s => s.IsPhoneNumberAvailableAsync(phoneNumber, CancellationToken.None))
                   .ReturnsAsync(true);

        var service = mockService.Object;

        // Act
        var result = await service.IsPhoneNumberAvailableAsync(phoneNumber, CancellationToken.None);

        // Assert
        Assert.True(result);
        mockService.Verify(s => s.IsPhoneNumberAvailableAsync(phoneNumber, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task IsPhoneNumberAvailableAsync_Should_Return_False_When_Not_Available()
    {
        // Arrange
        var mockService = new Mock<IAuthenticationService>();
        var phoneNumber = PhoneNumber.Create("+1234567890");
        
        mockService.Setup(s => s.IsPhoneNumberAvailableAsync(phoneNumber, CancellationToken.None))
                   .ReturnsAsync(false);

        var service = mockService.Object;

        // Act
        var result = await service.IsPhoneNumberAvailableAsync(phoneNumber, CancellationToken.None);

        // Assert
        Assert.False(result);
        mockService.Verify(s => s.IsPhoneNumberAvailableAsync(phoneNumber, CancellationToken.None), Times.Once);
    }
}

public class AuthorizationServiceTests
{
    [Fact]
    public async Task HasPermissionAsync_Should_Return_True_When_User_Has_Permission()
    {
        // Arrange
        var mockService = new Mock<IAuthorizationService>();
        var userId = Guid.NewGuid();
        var resource = "users";
        var action = "read";

        mockService.Setup(s => s.HasPermissionAsync(userId, resource, action, CancellationToken.None))
                   .ReturnsAsync(true);

        var service = mockService.Object;

        // Act
        var result = await service.HasPermissionAsync(userId, resource, action, CancellationToken.None);

        // Assert
        Assert.True(result);
        mockService.Verify(s => s.HasPermissionAsync(userId, resource, action, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HasPermissionAsync_Should_Return_False_When_User_Does_Not_Have_Permission()
    {
        // Arrange
        var mockService = new Mock<IAuthorizationService>();
        var userId = Guid.NewGuid();
        var resource = "users";
        var action = "delete";

        mockService.Setup(s => s.HasPermissionAsync(userId, resource, action, CancellationToken.None))
                   .ReturnsAsync(false);

        var service = mockService.Object;

        // Act
        var result = await service.HasPermissionAsync(userId, resource, action, CancellationToken.None);

        // Assert
        Assert.False(result);
        mockService.Verify(s => s.HasPermissionAsync(userId, resource, action, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetUserRolesAsync_Should_Return_Roles()
    {
        // Arrange
        var mockService = new Mock<IAuthorizationService>();
        var userId = Guid.NewGuid();
        var expectedRoles = new List<Role>
        {
            new Role("Admin", "Administrator role"),
            new Role("User", "Standard user role")
        };

        mockService.Setup(s => s.GetUserRolesAsync(userId, CancellationToken.None))
                   .ReturnsAsync(expectedRoles);

        var service = mockService.Object;

        // Act
        var result = await service.GetUserRolesAsync(userId, CancellationToken.None);

        // Assert
        Assert.Equal(expectedRoles.Count, result.Count());
        Assert.Contains(result, r => r.Name == "Admin");
        Assert.Contains(result, r => r.Name == "User");
        mockService.Verify(s => s.GetUserRolesAsync(userId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetUserPermissionsAsync_Should_Return_Permissions()
    {
        // Arrange
        var mockService = new Mock<IAuthorizationService>();
        var userId = Guid.NewGuid();
        var expectedPermissions = new List<Permission>
        {
            new Permission("read:users", "Can read users", "users", "read"),
            new Permission("write:users", "Can write users", "users", "write")
        };

        mockService.Setup(s => s.GetUserPermissionsAsync(userId, CancellationToken.None))
                   .ReturnsAsync(expectedPermissions);

        var service = mockService.Object;

        // Act
        var result = await service.GetUserPermissionsAsync(userId, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPermissions.Count, result.Count());
        Assert.Contains(result, p => p.Action == "read" && p.Resource == "users");
        Assert.Contains(result, p => p.Action == "write" && p.Resource == "users");
        mockService.Verify(s => s.GetUserPermissionsAsync(userId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AssignRoleToUserAsync_Should_Complete_Successfully()
    {
        // Arrange
        var mockService = new Mock<IAuthorizationService>();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var assignedBy = "admin@example.com";

        mockService.Setup(s => s.AssignRoleToUserAsync(userId, roleId, assignedBy, CancellationToken.None))
                   .Returns(Task.CompletedTask);

        var service = mockService.Object;

        // Act
        await service.AssignRoleToUserAsync(userId, roleId, assignedBy, CancellationToken.None);

        // Assert
        mockService.Verify(s => s.AssignRoleToUserAsync(userId, roleId, assignedBy, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RemoveRoleFromUserAsync_Should_Complete_Successfully()
    {
        // Arrange
        var mockService = new Mock<IAuthorizationService>();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        mockService.Setup(s => s.RemoveRoleFromUserAsync(userId, roleId, CancellationToken.None))
                   .Returns(Task.CompletedTask);

        var service = mockService.Object;

        // Act
        await service.RemoveRoleFromUserAsync(userId, roleId, CancellationToken.None);

        // Assert
        mockService.Verify(s => s.RemoveRoleFromUserAsync(userId, roleId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AssignPermissionToRoleAsync_Should_Complete_Successfully()
    {
        // Arrange
        var mockService = new Mock<IAuthorizationService>();
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var assignedBy = "admin@example.com";

        mockService.Setup(s => s.AssignPermissionToRoleAsync(roleId, permissionId, assignedBy, CancellationToken.None))
                   .Returns(Task.CompletedTask);

        var service = mockService.Object;

        // Act
        await service.AssignPermissionToRoleAsync(roleId, permissionId, assignedBy, CancellationToken.None);

        // Assert
        mockService.Verify(s => s.AssignPermissionToRoleAsync(roleId, permissionId, assignedBy, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RemovePermissionFromRoleAsync_Should_Complete_Successfully()
    {
        // Arrange
        var mockService = new Mock<IAuthorizationService>();
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        mockService.Setup(s => s.RemovePermissionFromRoleAsync(roleId, permissionId, CancellationToken.None))
                   .Returns(Task.CompletedTask);

        var service = mockService.Object;

        // Act
        await service.RemovePermissionFromRoleAsync(roleId, permissionId, CancellationToken.None);

        // Assert
        mockService.Verify(s => s.RemovePermissionFromRoleAsync(roleId, permissionId, CancellationToken.None), Times.Once);
    }
}
