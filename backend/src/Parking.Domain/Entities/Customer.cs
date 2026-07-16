namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Customer : AggregateRoot
{
    public long BranchId { get; private set; }
    public int CustomerType { get; private set; } // 1=Rotativo/Avulso, 2=Convenio, 3=Mensalista
    public string Name { get; private set; } = null!;
    public string Document { get; private set; } = null!; // CPF or CNPJ
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Customer() { }

    private Customer(
        long branchId,
        int customerType,
        string name,
        string document,
        string? phone,
        string? email) : base(0)
    {
        BranchId = branchId;
        CustomerType = customerType;
        Name = name;
        Document = document;
        Phone = phone;
        Email = email;
    }

    public static Result<Customer> Create(
        long branchId,
        int customerType,
        string name,
        string document,
        string? phone,
        string? email)
    {
        if (customerType is < 1 or > 3)
            return Result.Failure<Customer>(
                new Error("Customer.InvalidType", "Customer type must be 1 (Rotativo), 2 (Convenio) or 3 (Mensalista)."));

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Customer>(new Error("Customer.NameRequired", "Name is required."));

        if (string.IsNullOrWhiteSpace(document))
            return Result.Failure<Customer>(new Error("Customer.DocumentRequired", "Document is required."));

        return Result.Success(new Customer(branchId, customerType, name.Trim(), document.Trim(), phone, email));
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
