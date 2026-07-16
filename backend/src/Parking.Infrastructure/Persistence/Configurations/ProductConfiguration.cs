namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.BranchId).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(255).IsRequired();
        builder.Property(x => x.SKU).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.BranchId, x.SKU }).IsUnique();
        builder.Property(x => x.Category).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Cost).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.SellingPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Stock).HasColumnType("decimal(18,3)").IsRequired();
        builder.Property(x => x.MinimumStock).HasColumnType("decimal(18,3)").IsRequired();
        builder.Property(x => x.SupplierId);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
