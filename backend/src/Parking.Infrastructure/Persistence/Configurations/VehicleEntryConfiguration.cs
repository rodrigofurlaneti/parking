namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class VehicleEntryConfiguration : IEntityTypeConfiguration<VehicleEntry>
{
    public void Configure(EntityTypeBuilder<VehicleEntry> builder)
    {
        builder.ToTable("VehicleEntry");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.BranchId).IsRequired();
        builder.Property(x => x.ParkingSpaceId).IsRequired();
        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.LicensePlate).HasMaxLength(20).IsRequired();
        builder.Property(x => x.VehicleModel).HasMaxLength(255).IsRequired();
        builder.Property(x => x.VehicleColor).HasMaxLength(50).IsRequired();
        builder.Property(x => x.EntryTime).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.ExitTime).HasColumnType("datetime2");
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
