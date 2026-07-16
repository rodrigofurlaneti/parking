namespace Parking.Application.Features.ServiceCategory.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetAllServiceCategoriesByBranchQuery(long BranchId) : IQuery<List<ServiceCategoryDto>>;
