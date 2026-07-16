namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class WashOperationalCostConfiguration : IEntityTypeConfiguration<WashOperationalCost>
{
    public void Configure(EntityTypeBuilder<WashOperationalCost> builder)
    {
        builder.ToTable("WashOperationalCost");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.BranchId).IsRequired();
        builder.Property(x => x.MonthYear).HasColumnType("datetime2").IsRequired();
        builder.HasIndex(x => new { x.BranchId, x.MonthYear }).IsUnique();
        builder.Property(x => x.LaborCost).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.MaterialCost).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.EquipmentCost).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.UtilitiesCost).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TotalCost).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TotalRevenue).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        builder.Ignore(x => x.NetProfit);
        builder.Ignore(x => x.ProfitMargin);
    }
}
