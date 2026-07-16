namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class SalePayment : Entity
{
    public long SaleId { get; private set; }
    public int PaymentMethod { get; private set; } // 1=Cash,2=DebitCard,3=CreditCard,4=Pix,5=Agreement
    public decimal Amount { get; private set; }

    private SalePayment() { }

    private SalePayment(long saleId, int paymentMethod, decimal amount) : base(0)
    {
        SaleId = saleId;
        PaymentMethod = paymentMethod;
        Amount = amount;
    }

    public static Result<SalePayment> Create(long saleId, int paymentMethod, decimal amount)
    {
        if (amount <= 0)
            return Result.Failure<SalePayment>(
                new Error("SalePayment.InvalidAmount", "Amount must be greater than 0."));

        if (paymentMethod < 1 || paymentMethod > 5)
            return Result.Failure<SalePayment>(
                new Error("SalePayment.InvalidMethod", "Payment method must be between 1 and 5."));

        return Result.Success(new SalePayment(saleId, paymentMethod, amount));
    }
}
