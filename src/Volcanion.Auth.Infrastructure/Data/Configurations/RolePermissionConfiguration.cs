using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volcanion.Auth.Domain.Entities;

namespace Volcanion.Auth.Infrastructure.Data.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(rp => rp.Id);
        builder.Property(rp => rp.Id).ValueGeneratedNever();
        builder.Property(rp => rp.RoleId).IsRequired();
        builder.Property(rp => rp.PermissionId).IsRequired();
        builder.Property(rp => rp.AssignedAt).IsRequired();
        builder.Property(rp => rp.AssignedBy).HasMaxLength(100);
        builder.Property(rp => rp.CreatedAt).IsRequired();
        
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique().HasDatabaseName("IX_RolePermissions_RoleId_PermissionId");
        builder.HasQueryFilter(rp => !rp.IsDeleted);
    }
}
