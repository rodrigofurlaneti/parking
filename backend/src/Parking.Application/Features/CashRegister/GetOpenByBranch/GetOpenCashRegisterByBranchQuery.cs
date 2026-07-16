namespace Parking.Application.Features.CashRegister.GetOpenByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetOpenCashRegisterByBranchQuery(long BranchId) : IQuery<CashRegisterDto?>;
