namespace Parking.Application.Abstractions.Services;

public interface ICurrentUser
{
    long? Id { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}
