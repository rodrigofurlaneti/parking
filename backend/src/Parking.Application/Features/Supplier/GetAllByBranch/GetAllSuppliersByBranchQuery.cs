namespace Parking.Application.Features.Supplier.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetAllSuppliersByBranchQuery(long BranchId) : IQuery<List<SupplierDto>>;
