namespace Parking.Application.Features.Supplier.DeactivateSupplier;

using Parking.Application.Abstractions.Messaging;

public sealed record DeactivateSupplierCommand(long SupplierId) : ICommand;
