namespace Parking.Application.Abstractions.Services;

public interface ITokenService
{
    string GenerateAccessToken(long userId, string userName, string email);
    string? ValidateToken(string token);
}
