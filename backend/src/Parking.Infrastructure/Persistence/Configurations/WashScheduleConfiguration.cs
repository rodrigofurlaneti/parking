namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class WashScheduleConfiguration : IEntityTypeConfiguration<WashSchedule>
{
    public void Configure(EntityTypeBuilder<WashSchedule> builder)
    {
        builder.ToTable("WashSchedule");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.BranchId).IsRequired();
        builder.Property(x => x.VehicleEntryId).IsRequired();
        builder.Property(x => x.ScheduledTime).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.ActualStartTime).HasColumnType("datetime2");
        builder.Property(x => x.ActualEndTime).HasColumnType("datetime2");
        builder.Property(x => x.EmployeeId).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
