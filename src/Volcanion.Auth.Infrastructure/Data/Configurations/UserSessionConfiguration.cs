using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volcanion.Auth.Domain.Entities;

namespace Volcanion.Auth.Infrastructure.Data.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");
        builder.HasKey(us => us.Id);
        builder.Property(us => us.Id).ValueGeneratedNever();
        builder.Property(us => us.UserId).IsRequired();
        builder.Property(us => us.RefreshToken).HasMaxLength(500).IsRequired();
        builder.Property(us => us.DeviceInfo).HasMaxLength(200).IsRequired();
        builder.Property(us => us.IpAddress).HasMaxLength(45).IsRequired();
        builder.Property(us => us.UserAgent).HasMaxLength(1000).IsRequired();
        builder.Property(us => us.ExpiresAt).IsRequired();
        builder.Property(us => us.IsRevoked).IsRequired();
        builder.Property(us => us.RevokedAt);
        builder.Property(us => us.RevokedBy).HasMaxLength(100);
        builder.Property(us => us.LastAccessedAt).IsRequired();
        builder.Property(us => us.CreatedAt).IsRequired();
        
        builder.HasIndex(us => us.RefreshToken).IsUnique().HasDatabaseName("IX_UserSessions_RefreshToken");
        builder.HasIndex(us => us.UserId).HasDatabaseName("IX_UserSessions_UserId");
        builder.HasQueryFilter(us => !us.IsDeleted);
    }
}
