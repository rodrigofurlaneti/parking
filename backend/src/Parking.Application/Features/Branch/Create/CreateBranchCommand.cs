namespace Parking.Application.Features.Branch.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateBranchCommand(
    long CompanyId,
    string Name,
    string Address,
    int TotalSpaces) : ICommand<BranchDto>;
