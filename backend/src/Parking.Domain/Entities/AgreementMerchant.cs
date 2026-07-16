namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class AgreementMerchant : AggregateRoot
{
    public long BranchId { get; private set; }
    public string CompanyName { get; private set; } = null!;
    public decimal DiscountPercentage { get; private set; }
    public bool IsActive { get; private set; } = true;

    private AgreementMerchant() { }

    private AgreementMerchant(
        long branchId,
        string companyName,
        decimal discountPercentage) : base(0)
    {
        BranchId = branchId;
        CompanyName = companyName;
        DiscountPercentage = discountPercentage;
    }

    public static Result<AgreementMerchant> Create(
        long branchId,
        string companyName,
        decimal discountPercentage)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return Result.Failure<AgreementMerchant>(
                new Error("AgreementMerchant.NameRequired", "Company name is required."));

        if (discountPercentage < 0 || discountPercentage > 100)
            return Result.Failure<AgreementMerchant>(
                new Error("AgreementMerchant.InvalidDiscount", "Discount percentage must be between 0 and 100."));

        return Result.Success(new AgreementMerchant(branchId, companyName.Trim(), discountPercentage));
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
