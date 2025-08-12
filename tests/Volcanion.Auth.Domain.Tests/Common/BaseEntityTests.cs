using Volcanion.Auth.Domain.Common;

namespace Volcanion.Auth.Domain.Tests.Common;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity
    {
        public string Name { get; }

        public TestEntity(string name)
        {
            Name = name;
        }

        public TestEntity(Guid id, string name) : base(id)
        {
            Name = name;
        }
    }

    [Fact]
    public void BaseEntity_Constructor_Should_Set_Id_And_CreatedAt()
    {
        // Act
        var entity = new TestEntity("Test");

        // Assert
        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.True(entity.CreatedAt <= DateTime.UtcNow);
        Assert.False(entity.IsDeleted);
        Assert.Null(entity.UpdatedAt);
        Assert.Null(entity.CreatedBy);
        Assert.Null(entity.UpdatedBy);
        Assert.Null(entity.DeletedAt);
        Assert.Null(entity.DeletedBy);
    }

    [Fact]
    public void BaseEntity_Constructor_With_Id_Should_Use_Provided_Id()
    {
        // Arrange
        var specificId = Guid.NewGuid();

        // Act
        var entity = new TestEntity(specificId, "Test");

        // Assert
        Assert.Equal(specificId, entity.Id);
        Assert.True(entity.CreatedAt <= DateTime.UtcNow);
        Assert.False(entity.IsDeleted);
    }

    [Fact]
    public void BaseEntity_Should_Have_Different_Ids_By_Default()
    {
        // Arrange & Act
        var entity1 = new TestEntity("Test1");
        var entity2 = new TestEntity("Test2");

        // Assert
        Assert.NotEqual(entity1.Id, entity2.Id);
        Assert.NotEqual(entity1, entity2);
    }

    [Fact]
    public void BaseEntity_Should_Be_Same_Reference()
    {
        // Arrange
        var entity = new TestEntity("Test");

        // Act & Assert
        Assert.Equal(entity, entity);
        Assert.True(ReferenceEquals(entity, entity));
    }

    [Fact]
    public void MarkAsDeleted_Should_Set_Deletion_Properties()
    {
        // Arrange
        var entity = new TestEntity("Test");
        var deletedBy = "admin@example.com";

        // Act
        entity.MarkAsDeleted(deletedBy);

        // Assert
        Assert.True(entity.IsDeleted);
        Assert.NotNull(entity.DeletedAt);
        Assert.Equal(deletedBy, entity.DeletedBy);
        Assert.True(entity.DeletedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void MarkAsDeleted_Without_DeletedBy_Should_Set_Deletion_Properties()
    {
        // Arrange
        var entity = new TestEntity("Test");

        // Act
        entity.MarkAsDeleted();

        // Assert
        Assert.True(entity.IsDeleted);
        Assert.NotNull(entity.DeletedAt);
        Assert.Null(entity.DeletedBy);
        Assert.True(entity.DeletedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void MarkAsUpdated_Should_Set_Update_Properties()
    {
        // Arrange
        var entity = new TestEntity("Test");
        var updatedBy = "admin@example.com";

        // Act
        entity.MarkAsUpdated(updatedBy);

        // Assert
        Assert.NotNull(entity.UpdatedAt);
        Assert.Equal(updatedBy, entity.UpdatedBy);
        Assert.True(entity.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void MarkAsUpdated_Without_UpdatedBy_Should_Set_Update_Properties()
    {
        // Arrange
        var entity = new TestEntity("Test");

        // Act
        entity.MarkAsUpdated();

        // Assert
        Assert.NotNull(entity.UpdatedAt);
        Assert.Null(entity.UpdatedBy);
        Assert.True(entity.UpdatedAt <= DateTime.UtcNow);
    }
}

public class ValueObjectTests
{
    private class TestValueObject : ValueObject
    {
        public string Value { get; }

        public TestValueObject(string value)
        {
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }

    [Fact]
    public void ValueObject_Equality_Should_Work_With_Same_Values()
    {
        // Arrange
        var value1 = new TestValueObject("test");
        var value2 = new TestValueObject("test");

        // Act & Assert
        Assert.Equal(value1, value2);
        Assert.True(value1.Equals(value2));
        Assert.True(value1 == value2);
        Assert.False(value1 != value2);
    }

    [Fact]
    public void ValueObject_Equality_Should_Fail_With_Different_Values()
    {
        // Arrange
        var value1 = new TestValueObject("test1");
        var value2 = new TestValueObject("test2");

        // Act & Assert
        Assert.NotEqual(value1, value2);
        Assert.False(value1.Equals(value2));
        Assert.False(value1 == value2);
        Assert.True(value1 != value2);
    }

    [Fact]
    public void ValueObject_GetHashCode_Should_Be_Same_For_Equal_Values()
    {
        // Arrange
        var value1 = new TestValueObject("test");
        var value2 = new TestValueObject("test");

        // Act
        var hash1 = value1.GetHashCode();
        var hash2 = value2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ValueObject_GetHashCode_Should_Be_Different_For_Different_Values()
    {
        // Arrange
        var value1 = new TestValueObject("test1");
        var value2 = new TestValueObject("test2");

        // Act
        var hash1 = value1.GetHashCode();
        var hash2 = value2.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void ValueObject_Equality_With_Null_Should_Return_False()
    {
        // Arrange
        var value = new TestValueObject("test");

        // Act & Assert
        Assert.False(value.Equals(null));
        Assert.False(value! == null!);
        Assert.True(value! != null!);
    }

    [Fact]
    public void ValueObject_Equality_With_Different_Type_Should_Return_False()
    {
        // Arrange
        var value = new TestValueObject("test");
        var differentType = "test";

        // Act & Assert
        Assert.False(value.Equals(differentType));
    }

    [Fact]
    public void ValueObject_Null_Equality_Should_Work_Correctly()
    {
        // Arrange
        TestValueObject? value1 = null;
        TestValueObject? value2 = null;
        var value3 = new TestValueObject("test");

        // Act & Assert
        Assert.True(value1! == value2!);
        Assert.False(value1! != value2!);
        Assert.False(value1! == value3!);
        Assert.True(value1! != value3!);
        Assert.False(value3! == value1!);
        Assert.True(value3! != value1!);
    }
}
