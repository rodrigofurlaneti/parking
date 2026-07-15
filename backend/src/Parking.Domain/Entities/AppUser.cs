namespace Parking.Domain.Entities;

using Parking.Domain.Common;
using Parking.Domain.ValueObjects;

public sealed class AppUser : AggregateRoot
{
    public Username UserName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int FailedAccessCount { get; private set; }
    public DateTime? LockoutEndAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private AppUser() { }

    private AppUser(Username userName, Email email, string passwordHash, string fullName) : base(0)
    {
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
    }

    public static Result<AppUser> Create(Username userName, Email email, string passwordHash, string fullName)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure<AppUser>(new Error("AppUser.InvalidPassword", "Password hash is required."));

        return Result.Success(new AppUser(userName, email, passwordHash, fullName.Trim()));
    }

    public bool IsLockedOut => !IsActive && LockoutEndAt.HasValue && DateTime.UtcNow < LockoutEndAt.Value;

    public void IncrementFailedAccessCount()
    {
        FailedAccessCount++;
        if (FailedAccessCount >= 5)
        {
            IsActive = false;
            LockoutEndAt = DateTime.UtcNow.AddHours(1);
        }
    }

    public void ResetFailedAccessCount()
    {
        FailedAccessCount = 0;
        LockoutEndAt = null;
        IsActive = true;
    }

    public void SetLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void AssignRole(long roleId)
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
