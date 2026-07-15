namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class VehicleExitConfiguration : IEntityTypeConfiguration<VehicleExit>
{
    public void Configure(EntityTypeBuilder<VehicleExit> builder)
    {
        builder.ToTable("VehicleExit");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.VehicleEntryId).IsRequired();
        builder.Property(x => x.ExitTime).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.DurationMinutes).IsRequired();
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18, 2)").IsRequired();
        builder.Property(x => x.ParkingMode).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
