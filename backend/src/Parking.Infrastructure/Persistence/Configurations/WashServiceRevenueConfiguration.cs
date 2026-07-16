namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class WashServiceRevenueConfiguration : IEntityTypeConfiguration<WashServiceRevenue>
{
    public void Configure(EntityTypeBuilder<WashServiceRevenue> builder)
    {
        builder.ToTable("WashServiceRevenue");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.WashScheduleId).IsRequired();
        builder.Property(x => x.ServiceItemId).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Commission).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Date).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
