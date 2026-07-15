namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUser");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.OwnsOne(x => x.UserName, nav =>
        {
            nav.Property(x => x.Value).HasColumnName("UserName").HasMaxLength(80).IsRequired();
        });

        builder.OwnsOne(x => x.Email, nav =>
        {
            nav.Property(x => x.Value).HasColumnName("Email").HasMaxLength(255).IsRequired();
            nav.HasIndex(x => x.Value).IsUnique();
        });

        builder.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
        builder.Property(x => x.FullName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.FailedAccessCount).IsRequired();
        builder.Property(x => x.LockoutEndAt).HasColumnType("datetime2");
        builder.Property(x => x.LastLoginAt).HasColumnType("datetime2");
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
