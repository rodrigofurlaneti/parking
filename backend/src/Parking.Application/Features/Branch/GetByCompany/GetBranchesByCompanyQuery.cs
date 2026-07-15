namespace Parking.Application.Features.Branch.GetByCompany;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetBranchesByCompanyQuery(long CompanyId) : IQuery<List<BranchDto>>;
