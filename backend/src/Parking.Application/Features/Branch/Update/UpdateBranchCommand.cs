namespace Parking.Application.Features.Branch.Update;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record UpdateBranchCommand(
    long BranchId,
    string Name,
    string Address,
    string? PhoneNumber,
    int TotalSpaces) : ICommand<BranchDto>;
