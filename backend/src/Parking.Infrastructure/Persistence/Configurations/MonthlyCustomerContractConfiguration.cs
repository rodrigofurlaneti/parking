namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class MonthlyCustomerContractConfiguration : IEntityTypeConfiguration<MonthlyCustomerContract>
{
    public void Configure(EntityTypeBuilder<MonthlyCustomerContract> builder)
    {
        builder.ToTable("MonthlyCustomerContract");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.BranchId).IsRequired();
        builder.Property(x => x.MonthlyFee).HasColumnType("decimal(18, 2)").IsRequired();
        builder.Property(x => x.MaxVehicles).IsRequired();
        builder.Property(x => x.StartDate).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.EndDate).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
