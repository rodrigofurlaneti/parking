// ============================================================================
// PARKING DOMAIN — Base Classes & Primitives
// Namespace: Parking.Domain.Common
// ============================================================================

namespace Parking.Domain.Common;

using System;
using System.Collections.Generic;

/// <summary>
/// Base class for all entities in the domain.
/// Uses BIGINT identity (not int).
/// </summary>
public abstract class Entity : IEquatable<Entity>
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    protected Entity() { }

    protected Entity(long id)
    {
        Id = id;
    }

    public bool Equals(Entity? other)
    {
        return other is not null && Id == other.Id && GetType() == other.GetType();
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity entity && Equals(entity);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Base class for aggregate roots.
/// Can raise domain events.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot() { }

    protected AggregateRoot(long id) : base(id) { }

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}

/// <summary>
/// Marker interface for domain events.
/// Does NOT depend on MediatR in Domain layer.
/// </summary>
public interface IDomainEvent
{
}

/// <summary>
/// Base class for value objects.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract IEnumerable<object> GetAtomicValues();

    public bool Equals(ValueObject? other)
    {
        return other is not null && GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObject valueObject && Equals(valueObject);
    }

    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(default(HashCode), (hashCode, value) =>
            {
                hashCode.Add(value);
                return hashCode;
            })
            .ToHashCode();
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Immutable error record for railway-oriented programming.
/// </summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Error NullValue = new("General.Null", "Value cannot be null.");
    public static readonly Error NotFound = new("General.NotFound", "Resource not found.");
    public static readonly Error AlreadyExists = new("General.AlreadyExists", "Resource already exists.");
}

/// <summary>
/// Result type for railway-oriented programming.
/// Failure handling without exceptions for expected business errors.
/// </summary>
public abstract record Result
{
    public sealed record Success : Result;

    public sealed record Failure(Error Error) : Result;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public static Result Success() => new Success();
    public static Result Failure(Error error) => new Failure(error);
}

/// <summary>
/// Generic Result type with value.
/// </summary>
public abstract record Result<T>
{
    public sealed record Success(T Value) : Result<T>;

    public sealed record Failure(Error Error) : Result<T>;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public static Result<T> Success(T value) => new Success(value);
    public static Result<T> Failure(Error error) => new Failure(error);
}

// ============================================================================
// USAGE PATTERNS
// ============================================================================

/*
 * ENTITY FACTORY:
 *
 * public sealed class Company : AggregateRoot
 * {
 *     public string Name { get; private set; } = null!;
 *     public string Cnpj { get; private set; } = null!;
 *     public bool IsActive { get; private set; } = true;
 *
 *     private Company() { }  // EF Core only
 *
 *     private Company(long id, string name, string cnpj) : base(id)
 *     {
 *         Name = name;
 *         Cnpj = cnpj;
 *     }
 *
 *     public static Result<Company> Create(string name, string cnpj)
 *     {
 *         if (string.IsNullOrWhiteSpace(name))
 *             return Result.Failure<Company>(
 *                 new Error("Company.InvalidName", "Name is required."));
 *
 *         return Result.Success(new Company(0, name.Trim(), cnpj.Trim()));
 *     }
 *
 *     public void Deactivate()
 *     {
 *         IsActive = false;
 *         UpdatedAt = DateTime.UtcNow;
 *     }
 * }
 *
 * VALUE OBJECT EXAMPLE:
 *
 * public sealed class Email : ValueObject
 * {
 *     public string Value { get; }
 *
 *     private static readonly Regex EmailRegex = new(
 *         @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
 *         RegexOptions.Compiled | RegexOptions.IgnoreCase);
 *
 *     private Email(string value) => Value = value;
 *
 *     public static Result<Email> Create(string email)
 *     {
 *         if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
 *             return Result.Failure<Email>(
 *                 new Error("Email.Invalid", "Invalid email address."));
 *
 *         return Result.Success(new Email(email.Trim().ToLowerInvariant()));
 *     }
 *
 *     public override IEnumerable<object> GetAtomicValues() { yield return Value; }
 * }
 *
 * REPOSITORY INTERFACE:
 *
 * public interface ICompanyRepository
 * {
 *     Task<Company?> GetByIdAsync(long id, CancellationToken ct = default);
 *     Task<Company?> GetByCnpjAsync(string cnpj, CancellationToken ct = default);
 *     Task AddAsync(Company entity, CancellationToken ct = default);
 *     Task UpdateAsync(Company entity, CancellationToken ct = default);
 * }
 */
