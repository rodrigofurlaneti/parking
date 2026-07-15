namespace Parking.Application.Behaviors;

using MediatR;
using Microsoft.Extensions.Logging;

internal sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {CommandName}", typeof(TRequest).Name);
        try
        {
            var response = await next();
            logger.LogInformation("Handled {CommandName}", typeof(TRequest).Name);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling {CommandName}", typeof(TRequest).Name);
            throw;
        }
    }
}
