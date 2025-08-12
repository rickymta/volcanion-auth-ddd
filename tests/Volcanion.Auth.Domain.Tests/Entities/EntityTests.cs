using Volcanion.Auth.Domain.Entities;

namespace Volcanion.Auth.Domain.Tests.Entities;

public class RoleTests
{
    [Fact]
    public void Role_Constructor_Should_Set_Properties_Correctly()
    {
        // Arrange
        var roleName = "Admin";
        var description = "Administrator role";

        // Act
        var role = new Role(roleName, description);

        // Assert
        Assert.Equal(roleName, role.Name);
        Assert.Equal(description, role.Description);
        Assert.True(role.IsActive);
        Assert.NotEqual(Guid.Empty, role.Id);
        Assert.False(role.IsDeleted);
        Assert.True(role.CreatedAt <= DateTime.UtcNow);
        Assert.Empty(role.UserRoles);
        Assert.Empty(role.RolePermissions);
    }

    [Fact]
    public void Role_Constructor_Should_Throw_When_Name_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Role(null!, "Description"));
    }

    [Fact]
    public void Role_Constructor_Should_Throw_When_Name_Is_Empty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Role("", "Description"));
    }

    [Fact]
    public void Role_Constructor_Should_Throw_When_Name_Is_Whitespace()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Role("   ", "Description"));
    }

    [Fact]
    public void Role_Constructor_Should_Throw_When_Description_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Role("Admin", null!));
    }

    [Fact]
    public void Role_Constructor_Should_Throw_When_Description_Is_Empty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Role("Admin", ""));
    }

    [Fact]
    public void Role_Constructor_Should_Throw_When_Description_Is_Whitespace()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Role("Admin", "   "));
    }

    [Fact]
    public void Role_UpdateInfo_Should_Update_Properties()
    {
        // Arrange
        var role = new Role("Admin", "Administrator role");
        var newName = "SuperAdmin";
        var newDescription = "Super Administrator role";

        // Act
        role.UpdateInfo(newName, newDescription);

        // Assert
        Assert.Equal(newName, role.Name);
        Assert.Equal(newDescription, role.Description);
        Assert.NotNull(role.UpdatedAt);
        Assert.True(role.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Role_UpdateInfo_Should_Throw_When_Name_Is_Null()
    {
        // Arrange
        var role = new Role("Admin", "Administrator role");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => role.UpdateInfo(null!, "Description"));
    }

    [Fact]
    public void Role_UpdateInfo_Should_Throw_When_Description_Is_Null()
    {
        // Arrange
        var role = new Role("Admin", "Administrator role");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => role.UpdateInfo("NewName", null!));
    }

    [Fact]
    public void Role_Activate_Should_Set_IsActive_To_True()
    {
        // Arrange
        var role = new Role("Admin", "Administrator role");
        role.Deactivate(); // First deactivate it

        // Act
        role.Activate();

        // Assert
        Assert.True(role.IsActive);
        Assert.NotNull(role.UpdatedAt);
        Assert.True(role.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Role_Deactivate_Should_Set_IsActive_To_False()
    {
        // Arrange
        var role = new Role("Admin", "Administrator role");

        // Act
        role.Deactivate();

        // Assert
        Assert.False(role.IsActive);
        Assert.NotNull(role.UpdatedAt);
        Assert.True(role.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Role_UserRoles_Should_Be_ReadOnly()
    {
        // Arrange
        var role = new Role("Admin", "Administrator role");

        // Act & Assert
        Assert.IsAssignableFrom<ICollection<UserRole>>(role.UserRoles);
    }

    [Fact]
    public void Role_RolePermissions_Should_Be_ReadOnly()
    {
        // Arrange
        var role = new Role("Admin", "Administrator role");

        // Act & Assert
        Assert.IsAssignableFrom<ICollection<RolePermission>>(role.RolePermissions);
    }
}

public class PermissionTests
{
    [Fact]
    public void Permission_Constructor_Should_Set_Properties_Correctly()
    {
        // Arrange
        var permissionName = "ReadUsers";
        var description = "Can read users";
        var resource = "Users";
        var action = "Read";

        // Act
        var permission = new Permission(permissionName, description, resource, action);

        // Assert
        Assert.Equal(permissionName, permission.Name);
        Assert.Equal(description, permission.Description);
        Assert.Equal(resource, permission.Resource);
        Assert.Equal(action, permission.Action);
        Assert.True(permission.IsActive);
        Assert.NotEqual(Guid.Empty, permission.Id);
        Assert.False(permission.IsDeleted);
        Assert.True(permission.CreatedAt <= DateTime.UtcNow);
        Assert.Empty(permission.RolePermissions);
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Name_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission(null!, "Description", "Resource", "Action"));
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Name_Is_Empty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission("", "Description", "Resource", "Action"));
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Name_Is_Whitespace()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission("   ", "Description", "Resource", "Action"));
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Description_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission("ReadUsers", null!, "Resource", "Action"));
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Description_Is_Empty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission("ReadUsers", "", "Resource", "Action"));
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Description_Is_Whitespace()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission("ReadUsers", "   ", "Resource", "Action"));
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Resource_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission("ReadUsers", "Description", null!, "Action"));
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Resource_Is_Empty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission("ReadUsers", "Description", "", "Action"));
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Resource_Is_Whitespace()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission("ReadUsers", "Description", "   ", "Action"));
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Action_Is_Null()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission("ReadUsers", "Description", "Resource", null!));
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Action_Is_Empty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission("ReadUsers", "Description", "Resource", ""));
    }

    [Fact]
    public void Permission_Constructor_Should_Throw_When_Action_Is_Whitespace()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission("ReadUsers", "Description", "Resource", "   "));
    }

    [Fact]
    public void Permission_UpdateInfo_Should_Update_Properties()
    {
        // Arrange
        var permission = new Permission("ReadUsers", "Can read users", "Users", "Read");
        var newName = "ReadAllUsers";
        var newDescription = "Can read all users";
        var newResource = "AllUsers";
        var newAction = "ReadAll";

        // Act
        permission.UpdateInfo(newName, newDescription, newResource, newAction);

        // Assert
        Assert.Equal(newName, permission.Name);
        Assert.Equal(newDescription, permission.Description);
        Assert.Equal(newResource, permission.Resource);
        Assert.Equal(newAction, permission.Action);
        Assert.NotNull(permission.UpdatedAt);
        Assert.True(permission.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Permission_UpdateInfo_Should_Throw_When_Name_Is_Null()
    {
        // Arrange
        var permission = new Permission("ReadUsers", "Can read users", "Users", "Read");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => permission.UpdateInfo(null!, "Description", "Resource", "Action"));
    }

    [Fact]
    public void Permission_UpdateInfo_Should_Throw_When_Description_Is_Null()
    {
        // Arrange
        var permission = new Permission("ReadUsers", "Can read users", "Users", "Read");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => permission.UpdateInfo("Name", null!, "Resource", "Action"));
    }

    [Fact]
    public void Permission_UpdateInfo_Should_Throw_When_Resource_Is_Null()
    {
        // Arrange
        var permission = new Permission("ReadUsers", "Can read users", "Users", "Read");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => permission.UpdateInfo("Name", "Description", null!, "Action"));
    }

    [Fact]
    public void Permission_UpdateInfo_Should_Throw_When_Action_Is_Null()
    {
        // Arrange
        var permission = new Permission("ReadUsers", "Can read users", "Users", "Read");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => permission.UpdateInfo("Name", "Description", "Resource", null!));
    }

    [Fact]
    public void Permission_Activate_Should_Set_IsActive_To_True()
    {
        // Arrange
        var permission = new Permission("ReadUsers", "Can read users", "Users", "Read");
        permission.Deactivate(); // First deactivate it

        // Act
        permission.Activate();

        // Assert
        Assert.True(permission.IsActive);
        Assert.NotNull(permission.UpdatedAt);
        Assert.True(permission.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Permission_Deactivate_Should_Set_IsActive_To_False()
    {
        // Arrange
        var permission = new Permission("ReadUsers", "Can read users", "Users", "Read");

        // Act
        permission.Deactivate();

        // Assert
        Assert.False(permission.IsActive);
        Assert.NotNull(permission.UpdatedAt);
        Assert.True(permission.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Permission_RolePermissions_Should_Be_ReadOnly()
    {
        // Arrange
        var permission = new Permission("ReadUsers", "Can read users", "Users", "Read");

        // Act & Assert
        Assert.IsAssignableFrom<ICollection<RolePermission>>(permission.RolePermissions);
    }
}

public class UserRoleTests
{
    [Fact]
    public void UserRole_Constructor_Should_Set_Properties_Correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var assignedBy = "TestUser";

        // Act
        var userRole = new UserRole(userId, roleId, assignedBy);

        // Assert
        Assert.Equal(userId, userRole.UserId);
        Assert.Equal(roleId, userRole.RoleId);
        Assert.Equal(assignedBy, userRole.AssignedBy);
        Assert.True(userRole.AssignedAt <= DateTime.UtcNow);
        Assert.NotEqual(Guid.Empty, userRole.Id);
        Assert.False(userRole.IsDeleted);
        Assert.True(userRole.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void UserRole_Constructor_Without_AssignedBy_Should_Set_Properties_Correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        // Act
        var userRole = new UserRole(userId, roleId);

        // Assert
        Assert.Equal(userId, userRole.UserId);
        Assert.Equal(roleId, userRole.RoleId);
        Assert.Null(userRole.AssignedBy);
        Assert.True(userRole.AssignedAt <= DateTime.UtcNow);
        Assert.NotEqual(Guid.Empty, userRole.Id);
        Assert.False(userRole.IsDeleted);
        Assert.True(userRole.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void UserRole_Constructor_Should_Accept_Empty_Guids()
    {
        // Arrange
        var userId = Guid.Empty;
        var roleId = Guid.Empty;

        // Act
        var userRole = new UserRole(userId, roleId);

        // Assert
        Assert.Equal(userId, userRole.UserId);
        Assert.Equal(roleId, userRole.RoleId);
        Assert.True(userRole.AssignedAt <= DateTime.UtcNow);
    }
}

public class UserSessionTests
{
    [Fact]
    public void UserSession_Constructor_Should_Set_Properties_Correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var refreshToken = "refresh-token-123";
        var deviceInfo = "iPhone 12";
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var expiresAt = DateTime.UtcNow.AddDays(7);

        // Act
        var session = new UserSession(userId, refreshToken, deviceInfo, ipAddress, userAgent, expiresAt);

        // Assert
        Assert.Equal(userId, session.UserId);
        Assert.Equal(refreshToken, session.RefreshToken);
        Assert.Equal(deviceInfo, session.DeviceInfo);
        Assert.Equal(ipAddress, session.IpAddress);
        Assert.Equal(userAgent, session.UserAgent);
        Assert.Equal(expiresAt, session.ExpiresAt);
        Assert.False(session.IsRevoked);
        Assert.Null(session.RevokedAt);
        Assert.Null(session.RevokedBy);
        Assert.True(session.LastAccessedAt <= DateTime.UtcNow);
        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.False(session.IsDeleted);
        Assert.True(session.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void UserSession_Constructor_Should_Throw_When_RefreshToken_Is_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserSession(userId, null!, "device", "ip", "agent", expiresAt));
    }

    [Fact]
    public void UserSession_Constructor_Should_Throw_When_DeviceInfo_Is_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserSession(userId, "token", null!, "ip", "agent", expiresAt));
    }

    [Fact]
    public void UserSession_Constructor_Should_Throw_When_IpAddress_Is_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserSession(userId, "token", "device", null!, "agent", expiresAt));
    }

    [Fact]
    public void UserSession_Constructor_Should_Throw_When_UserAgent_Is_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserSession(userId, "token", "device", "ip", null!, expiresAt));
    }

    [Fact]
    public void UserSession_UpdateLastAccess_Should_Update_LastAccessedAt()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "token", "device", "ip", "agent", DateTime.UtcNow.AddDays(7));
        var originalLastAccess = session.LastAccessedAt;
        Thread.Sleep(10); // Ensure time difference

        // Act
        session.UpdateLastAccess();

        // Assert
        Assert.True(session.LastAccessedAt > originalLastAccess);
        Assert.NotNull(session.UpdatedAt);
        Assert.True(session.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void UserSession_Revoke_Should_Set_Revoked_Properties()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "token", "device", "ip", "agent", DateTime.UtcNow.AddDays(7));
        var revokedBy = "admin";

        // Act
        session.Revoke(revokedBy);

        // Assert
        Assert.True(session.IsRevoked);
        Assert.Equal(revokedBy, session.RevokedBy);
        Assert.NotNull(session.RevokedAt);
        Assert.True(session.RevokedAt <= DateTime.UtcNow);
        Assert.NotNull(session.UpdatedAt);
        Assert.True(session.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void UserSession_Revoke_Without_RevokedBy_Should_Set_Revoked_Properties()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "token", "device", "ip", "agent", DateTime.UtcNow.AddDays(7));

        // Act
        session.Revoke();

        // Assert
        Assert.True(session.IsRevoked);
        Assert.Null(session.RevokedBy);
        Assert.NotNull(session.RevokedAt);
        Assert.True(session.RevokedAt <= DateTime.UtcNow);
        Assert.NotNull(session.UpdatedAt);
    }

    [Fact]
    public void UserSession_IsExpired_Should_Return_True_When_Expired()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "token", "device", "ip", "agent", DateTime.UtcNow.AddMinutes(-1));

        // Act
        var isExpired = session.IsExpired();

        // Assert
        Assert.True(isExpired);
    }

    [Fact]
    public void UserSession_IsExpired_Should_Return_False_When_Not_Expired()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "token", "device", "ip", "agent", DateTime.UtcNow.AddDays(7));

        // Act
        var isExpired = session.IsExpired();

        // Assert
        Assert.False(isExpired);
    }

    [Fact]
    public void UserSession_IsValid_Should_Return_True_When_Not_Revoked_And_Not_Expired()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "token", "device", "ip", "agent", DateTime.UtcNow.AddDays(7));

        // Act
        var isValid = session.IsValid();

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void UserSession_IsValid_Should_Return_False_When_Revoked()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "token", "device", "ip", "agent", DateTime.UtcNow.AddDays(7));
        session.Revoke();

        // Act
        var isValid = session.IsValid();

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void UserSession_IsValid_Should_Return_False_When_Expired()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "token", "device", "ip", "agent", DateTime.UtcNow.AddMinutes(-1));

        // Act
        var isValid = session.IsValid();

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void UserSession_IsValid_Should_Return_False_When_Both_Revoked_And_Expired()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "token", "device", "ip", "agent", DateTime.UtcNow.AddMinutes(-1));
        session.Revoke();

        // Act
        var isValid = session.IsValid();

        // Assert
        Assert.False(isValid);
    }
}

public class RolePermissionTests
{
    [Fact]
    public void RolePermission_Constructor_Should_Set_Properties_Correctly()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var assignedBy = "TestUser";

        // Act
        var rolePermission = new RolePermission(roleId, permissionId, assignedBy);

        // Assert
        Assert.Equal(roleId, rolePermission.RoleId);
        Assert.Equal(permissionId, rolePermission.PermissionId);
        Assert.Equal(assignedBy, rolePermission.AssignedBy);
        Assert.True(rolePermission.AssignedAt <= DateTime.UtcNow);
        Assert.NotEqual(Guid.Empty, rolePermission.Id);
        Assert.False(rolePermission.IsDeleted);
        Assert.True(rolePermission.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void RolePermission_Constructor_Without_AssignedBy_Should_Set_Properties_Correctly()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        // Act
        var rolePermission = new RolePermission(roleId, permissionId);

        // Assert
        Assert.Equal(roleId, rolePermission.RoleId);
        Assert.Equal(permissionId, rolePermission.PermissionId);
        Assert.Null(rolePermission.AssignedBy);
        Assert.True(rolePermission.AssignedAt <= DateTime.UtcNow);
        Assert.NotEqual(Guid.Empty, rolePermission.Id);
        Assert.False(rolePermission.IsDeleted);
        Assert.True(rolePermission.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void RolePermission_Constructor_Should_Accept_Empty_Guids()
    {
        // Arrange
        var roleId = Guid.Empty;
        var permissionId = Guid.Empty;

        // Act
        var rolePermission = new RolePermission(roleId, permissionId);

        // Assert
        Assert.Equal(roleId, rolePermission.RoleId);
        Assert.Equal(permissionId, rolePermission.PermissionId);
        Assert.True(rolePermission.AssignedAt <= DateTime.UtcNow);
    }
}