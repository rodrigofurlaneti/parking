namespace Parking.Application.Abstractions.Messaging;

using MediatR;
using Parking.Domain.Common;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse> { }
