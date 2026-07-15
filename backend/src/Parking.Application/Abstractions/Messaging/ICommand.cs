namespace Parking.Application.Abstractions.Messaging;

using MediatR;
using Parking.Domain.Common;

public interface ICommand : IRequest<Result> { }
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
