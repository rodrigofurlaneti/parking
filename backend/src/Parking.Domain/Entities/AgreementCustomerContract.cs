namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class AgreementCustomerContract : Entity
{
    public long CustomerId { get; private set; }
    public long AgreementMerchantId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private AgreementCustomerContract() { }

    private AgreementCustomerContract(
        long customerId,
        long agreementMerchantId,
        DateTime startDate,
        DateTime endDate) : base(0)
    {
        CustomerId = customerId;
        AgreementMerchantId = agreementMerchantId;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Result<AgreementCustomerContract> Create(
        long customerId,
        long agreementMerchantId,
        DateTime startDate,
        DateTime endDate)
    {
        if (endDate <= startDate)
            return Result.Failure<AgreementCustomerContract>(
                new Error("AgreementCustomerContract.InvalidPeriod", "End date must be after start date."));

        return Result.Success(new AgreementCustomerContract(customerId, agreementMerchantId, startDate, endDate));
    }

    public bool IsValidOn(DateTime date)
    {
        return IsActive && StartDate <= date && date <= EndDate;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
