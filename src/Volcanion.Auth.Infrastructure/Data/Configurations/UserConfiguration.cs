using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.ValueObjects;

namespace Volcanion.Auth.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(254)
                .IsRequired();
            
            email.HasIndex(e => e.Value)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");
        });

        builder.OwnsOne(u => u.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20);
            
            phone.HasIndex(p => p.Value)
                .IsUnique()
                .HasDatabaseName("IX_Users_PhoneNumber");
        });

        builder.OwnsOne(u => u.Password, password =>
        {
            password.Property(p => p.HashedValue)
                .HasColumnName("PasswordHash")
                .HasMaxLength(255)
                .IsRequired();
        });

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(u => u.IsEmailVerified)
            .IsRequired();

        builder.Property(u => u.IsPhoneVerified)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.LastLoginAt);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(100);

        builder.Property(u => u.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(u => u.IsDeleted)
            .IsRequired();

        builder.Property(u => u.DeletedAt);

        builder.Property(u => u.DeletedBy)
            .HasMaxLength(100);

        // Relationships
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserSessions)
            .WithOne(us => us.User)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Soft delete filter
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
