using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volcanion.Auth.Domain.Entities;

namespace Volcanion.Auth.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500).IsRequired();
        builder.Property(p => p.Resource).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Action).HasMaxLength(50).IsRequired();
        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
        
        builder.HasIndex(p => p.Name).IsUnique().HasDatabaseName("IX_Permissions_Name");
        builder.HasIndex(p => new { p.Resource, p.Action }).IsUnique().HasDatabaseName("IX_Permissions_Resource_Action");
        builder.HasQueryFilter(p => !p.IsDeleted);
        
        builder.HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
