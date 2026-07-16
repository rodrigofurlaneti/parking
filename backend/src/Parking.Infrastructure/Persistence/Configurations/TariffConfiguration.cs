namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class TariffConfiguration : IEntityTypeConfiguration<Tariff>
{
    public void Configure(EntityTypeBuilder<Tariff> builder)
    {
        builder.ToTable("Tariff");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.BranchId).IsRequired();
        builder.Property(x => x.FirstHourRate).HasColumnType("decimal(10, 2)").IsRequired();
        builder.Property(x => x.AdditionalHourRate).HasColumnType("decimal(10, 2)").IsRequired();
        builder.Property(x => x.DailyMaxRate).HasColumnType("decimal(10, 2)");
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");

        builder.HasIndex(x => new { x.BranchId, x.IsActive });
    }
}
