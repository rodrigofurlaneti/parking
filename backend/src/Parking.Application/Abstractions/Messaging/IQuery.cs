namespace Parking.Application.Abstractions.Messaging;

using MediatR;
using Parking.Domain.Common;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
