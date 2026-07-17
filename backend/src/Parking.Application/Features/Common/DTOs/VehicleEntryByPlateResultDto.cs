namespace Parking.Application.Features.Common.DTOs;

public sealed record VehicleEntryByPlateResultDto(
    long Id,
    long BranchId,
    long ParkingSpaceId,
    long CustomerId,
    string CustomerName,
    int CustomerType,
    bool IsNewCustomer,
    string LicensePlate,
    string VehicleModel,
    string VehicleColor,
    DateTime EntryTime,
    DateTime? ExitTime,
    int Status,
    bool IsActive);
