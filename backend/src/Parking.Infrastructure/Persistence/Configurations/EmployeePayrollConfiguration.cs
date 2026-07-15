namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class EmployeePayrollConfiguration : IEntityTypeConfiguration<EmployeePayroll>
{
    public void Configure(EntityTypeBuilder<EmployeePayroll> builder)
    {
        builder.ToTable("EmployeePayroll");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.EmployeeId).IsRequired();
        builder.Property(x => x.MonthYear).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.BaseSalary).HasColumnType("decimal(18, 2)").IsRequired();
        builder.Property(x => x.Bonuses).HasColumnType("decimal(18, 2)").IsRequired();
        builder.Property(x => x.Deductions).HasColumnType("decimal(18, 2)").IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.PaidDate).HasColumnType("datetime2");
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}
