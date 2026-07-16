namespace Parking.Application.Features.Supplier.CreateSupplier;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateSupplierCommand(
    long BranchId,
    string Name,
    string Document,
    string? Phone,
    string? Email) : ICommand<SupplierDto>;
