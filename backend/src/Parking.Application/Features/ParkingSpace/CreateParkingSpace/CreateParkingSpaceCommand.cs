namespace Parking.Application.Features.ParkingSpace.CreateParkingSpace;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateParkingSpaceCommand(
    long BranchId,
    string SpaceNumber,
    int Type) : ICommand<ParkingSpaceDto>;
