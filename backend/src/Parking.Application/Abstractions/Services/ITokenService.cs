namespace Parking.Application.Abstractions.Services;

public interface ITokenService
{
    string GenerateAccessToken(long userId, string userName, string email, IEnumerable<string>? roles = null);
    string? ValidateToken(string token);
}
