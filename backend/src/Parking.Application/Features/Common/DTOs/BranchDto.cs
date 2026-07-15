namespace Parking.Application.Features.Common.DTOs;

public sealed record BranchDto(
    long Id,
    long CompanyId,
    string Name,
    int TotalSpaces,
    bool IsActive);
