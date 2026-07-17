namespace Parking.Application.Features.Supplier.UpdateSupplier;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record UpdateSupplierCommand(
    long SupplierId,
    string Name,
    string Document,
    string? Phone,
    string? Email) : ICommand<SupplierDto>;
