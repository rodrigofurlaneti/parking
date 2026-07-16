namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class WashEmployeeConfiguration : IEntityTypeConfiguration<WashEmployee>
{
    public void Configure(EntityTypeBuilder<WashEmployee> builder)
    {
        builder.ToTable("WashEmployee");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.EmployeeId).IsRequired();
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.Property(x => x.Specializations).HasMaxLength(500);
        builder.Property(x => x.Certifications).HasMaxLength(500);
        builder.Property(x => x.TrainingLevel).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
