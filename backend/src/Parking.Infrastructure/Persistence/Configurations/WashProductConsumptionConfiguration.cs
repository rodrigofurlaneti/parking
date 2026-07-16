namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class WashProductConsumptionConfiguration : IEntityTypeConfiguration<WashProductConsumption>
{
    public void Configure(EntityTypeBuilder<WashProductConsumption> builder)
    {
        builder.ToTable("WashProductConsumption");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.WashScheduleId).IsRequired();
        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.QuantityUsed).HasColumnType("decimal(18,3)").IsRequired();
        builder.Property(x => x.UnitCost).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TotalCost).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
