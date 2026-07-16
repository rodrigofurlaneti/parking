namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class AgreementCustomerContractConfiguration : IEntityTypeConfiguration<AgreementCustomerContract>
{
    public void Configure(EntityTypeBuilder<AgreementCustomerContract> builder)
    {
        builder.ToTable("AgreementCustomerContract");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.AgreementMerchantId).IsRequired();
        builder.Property(x => x.StartDate).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.EndDate).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
