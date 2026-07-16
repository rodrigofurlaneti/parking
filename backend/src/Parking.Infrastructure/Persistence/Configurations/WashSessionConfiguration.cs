namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class WashSessionConfiguration : IEntityTypeConfiguration<WashSession>
{
    public void Configure(EntityTypeBuilder<WashSession> builder)
    {
        builder.ToTable("WashSession");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.WashScheduleId).IsRequired();
        builder.Property(x => x.TotalCost).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TotalDurationMinutes).IsRequired();
        builder.Property(x => x.StartTime).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.EndTime).HasColumnType("datetime2");
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
