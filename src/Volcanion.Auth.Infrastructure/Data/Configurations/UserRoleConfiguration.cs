using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volcanion.Auth.Domain.Entities;

namespace Volcanion.Auth.Infrastructure.Data.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasKey(ur => ur.Id);
        builder.Property(ur => ur.Id).ValueGeneratedNever();
        builder.Property(ur => ur.UserId).IsRequired();
        builder.Property(ur => ur.RoleId).IsRequired();
        builder.Property(ur => ur.AssignedAt).IsRequired();
        builder.Property(ur => ur.AssignedBy).HasMaxLength(100);
        builder.Property(ur => ur.CreatedAt).IsRequired();
        
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique().HasDatabaseName("IX_UserRoles_UserId_RoleId");
        builder.HasQueryFilter(ur => !ur.IsDeleted);
    }
}
