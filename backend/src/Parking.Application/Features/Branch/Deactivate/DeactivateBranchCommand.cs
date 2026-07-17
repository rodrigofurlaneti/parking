namespace Parking.Application.Features.Branch.Deactivate;

using Parking.Application.Abstractions.Messaging;

public sealed record DeactivateBranchCommand(long BranchId) : ICommand;
