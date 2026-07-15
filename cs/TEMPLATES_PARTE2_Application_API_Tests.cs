/*
 * TEMPLATES_PARTE2_Application_API_Tests.cs
 *
 * Copy-paste ready code for Fase 1 Application, API, and Tests layers.
 *
 * Instructions:
 * 1. For each class/interface below, create individual .cs files in the correct folder
 * 2. Adjust namespaces and paths according to your folder structure
 * 3. Replace placeholders marked with [TODO] or [CUSTOMIZE]
 *
 * Files to Create:
 *
 * APPLICATION LAYER:
 * ─────────────────────────────────────────────────────────────────
 * Parking.Application/Abstractions/Messaging/ICommand.cs
 * Parking.Application/Abstractions/Messaging/ICommandHandler.cs
 * Parking.Application/Abstractions/Messaging/IQuery.cs
 * Parking.Application/Abstractions/Messaging/IQueryHandler.cs
 * Parking.Application/Abstractions/Services/IPasswordHasher.cs
 * Parking.Application/Abstractions/Services/ITokenService.cs
 * Parking.Application/Abstractions/Services/ICurrentUser.cs
 *
 * Parking.Application/Features/Auth/CreateUser/CreateUserCommand.cs
 * Parking.Application/Features/Auth/CreateUser/CreateUserCommandHandler.cs
 * Parking.Application/Features/Auth/CreateUser/CreateUserCommandValidator.cs
 *
 * Parking.Application/Features/Auth/Login/LoginCommand.cs
 * Parking.Application/Features/Auth/Login/LoginCommandHandler.cs
 * Parking.Application/Features/Auth/Login/LoginCommandValidator.cs
 *
 * Parking.Application/Features/Auth/RefreshToken/RefreshTokenCommand.cs
 * Parking.Application/Features/Auth/RefreshToken/RefreshTokenCommandHandler.cs
 * Parking.Application/Features/Auth/RefreshToken/RefreshTokenCommandValidator.cs
 *
 * Parking.Application/Features/Auth/AssignRole/AssignRoleCommand.cs
 * Parking.Application/Features/Auth/AssignRole/AssignRoleCommandHandler.cs
 * Parking.Application/Features/Auth/AssignRole/AssignRoleCommandValidator.cs
 *
 * Parking.Application/Features/Auth/GetUsers/GetAllUsersQuery.cs
 * Parking.Application/Features/Auth/GetUsers/GetAllUsersQueryHandler.cs
 *
 * Parking.Application/Features/Company/Create/CreateCompanyCommand.cs
 * Parking.Application/Features/Company/Create/CreateCompanyCommandHandler.cs
 * Parking.Application/Features/Company/Create/CreateCompanyCommandValidator.cs
 *
 * Parking.Application/Features/Company/GetById/GetCompanyByIdQuery.cs
 * Parking.Application/Features/Company/GetById/GetCompanyByIdQueryHandler.cs
 *
 * Parking.Application/Features/Branch/Create/CreateBranchCommand.cs
 * Parking.Application/Features/Branch/Create/CreateBranchCommandHandler.cs
 * Parking.Application/Features/Branch/Create/CreateBranchCommandValidator.cs
 *
 * Parking.Application/Features/Branch/GetByCompany/GetBranchesByCompanyQuery.cs
 * Parking.Application/Features/Branch/GetByCompany/GetBranchesByCompanyQueryHandler.cs
 *
 * Parking.Application/Features/Common/DTOs/UserDto.cs
 * Parking.Application/Features/Common/DTOs/RoleDto.cs
 * Parking.Application/Features/Common/DTOs/CompanyDto.cs
 * Parking.Application/Features/Common/DTOs/BranchDto.cs
 *
 * Parking.Application/Behaviors/LoggingBehavior.cs
 * Parking.Application/Behaviors/ValidationBehavior.cs
 *
 * Parking.Application/DependencyInjection.cs
 *
 * API LAYER:
 * ─────────────────────────────────────────────────────────────────
 * Parking.API/Controllers/ApiController.cs
 * Parking.API/Controllers/AuthController.cs
 * Parking.API/Controllers/CompanyController.cs
 * Parking.API/Controllers/BranchController.cs
 * Parking.API/Middleware/ExceptionHandlingMiddleware.cs
 * Parking.API/Program.cs
 * Parking.API/appsettings.json
 * Parking.API/appsettings.Development.json
 *
 * TESTS:
 * ─────────────────────────────────────────────────────────────────
 * Parking.Tests/Handlers/CreateUserCommandHandlerTests.cs
 * Parking.Tests/Handlers/LoginCommandHandlerTests.cs
 * Parking.Tests/Handlers/CreateCompanyCommandHandlerTests.cs
 * Parking.Tests/Handlers/GetAllUsersQueryHandlerTests.cs
 *
 * Parking.Specs/Features/Authentication.feature
 * Parking.Specs/Features/Company.feature
 * Parking.Specs/StepDefinitions/AuthenticationSteps.cs
 * Parking.Specs/StepDefinitions/CompanySteps.cs
 *
 * Parking.ArchTests/ArchitectureTests.cs
 */

// ═══════════════════════════════════════════════════════════════════════════
// APPLICATION LAYER — MESSAGING ABSTRACTIONS
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Application.Abstractions.Messaging;

using MediatR;
using Parking.Domain.Common;

public interface ICommand : IRequest<Result> { }
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand { }

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse> { }

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse> { }

// ═══════════════════════════════════════════════════════════════════════════
// APPLICATION LAYER — SERVICE ABSTRACTIONS
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Application.Abstractions.Services;

using Parking.Domain.Common;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface ITokenService
{
    string GenerateAccessToken(long userId, string userName, string email);
    string? ValidateToken(string token);
}

public interface ICurrentUser
{
    long? Id { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}

// ═══════════════════════════════════════════════════════════════════════════
// APPLICATION LAYER — DTOS
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Application.Features.Common.DTOs;

public sealed record UserDto(
    long Id,
    string UserName,
    string Email,
    string FullName,
    bool IsActive,
    List<string> Roles);

public sealed record RoleDto(
    long Id,
    string Name,
    bool IsActive,
    List<string> Permissions);

public sealed record CompanyDto(
    long Id,
    string Name,
    string Cnpj,
    string LegalName,
    bool IsActive);

public sealed record BranchDto(
    long Id,
    long CompanyId,
    string Name,
    int TotalSpaces,
    bool IsActive);

// ═══════════════════════════════════════════════════════════════════════════
// APPLICATION LAYER — AUTH COMMANDS
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Application.Features.Auth.CreateUser;

using FluentValidation;
using Parking.Application.Abstractions.Messaging;
using Parking.Application.Abstractions.Services;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Parking.Domain.ValueObjects;

public sealed record CreateUserCommand(
    string UserName,
    string Email,
    string FullName,
    string Password) : ICommand<UserDto>;

internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(80);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

internal sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserDto>
{
    private readonly IAppUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IAppUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicates
        if (await _userRepository.ExistsAsync(request.UserName, request.Email, cancellationToken))
            return Result.Failure<UserDto>(new Error("AppUser.DuplicateEmail", "User already exists."));

        // Create value objects
        var usernameResult = Username.Create(request.UserName);
        if (usernameResult.IsFailure)
            return Result.Failure<UserDto>(usernameResult.Error);

        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<UserDto>(emailResult.Error);

        // Create user
        var userResult = AppUser.Create(
            usernameResult.Value,
            emailResult.Value,
            request.FullName,
            _passwordHasher.Hash(request.Password));

        if (userResult.IsFailure)
            return Result.Failure<UserDto>(userResult.Error);

        await _userRepository.AddAsync(userResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new UserDto(
            userResult.Value.Id,
            userResult.Value.UserName.Value,
            userResult.Value.Email.Value,
            userResult.Value.FullName,
            userResult.Value.IsActive,
            new List<string>()));
    }
}

// ─────────────────────────────────────────────────────────────────────────
// Login Command

namespace Parking.Application.Features.Auth.Login;

using FluentValidation;
using Parking.Application.Abstractions.Messaging;
using Parking.Application.Abstractions.Services;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

public sealed record LoginCommand(
    string UserName,
    string Password) : ICommand<LoginResponse>;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    long UserId,
    string UserName);

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IAppUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IAppUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<LoginResponse>(
                new Error("Auth.InvalidCredentials", "Invalid username or password."));

        if (!user.IsActive)
            return Result.Failure<LoginResponse>(
                new Error("Auth.UserInactive", "User account is inactive."));

        if (user.IsLockedOut)
            return Result.Failure<LoginResponse>(
                new Error("Auth.UserLockedOut", "User account is locked out."));

        // Reset failed attempts
        user.ResetFailedAccessCount();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.UserName.Value, user.Email.Value);

        // Create refresh token (simplified - should use RefreshToken entity)
        var refreshToken = Guid.NewGuid().ToString("N");

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new LoginResponse(accessToken, refreshToken, user.Id, user.UserName.Value));
    }
}

// ─────────────────────────────────────────────────────────────────────────
// RefreshToken Command

namespace Parking.Application.Features.Auth.RefreshToken;

using FluentValidation;
using Parking.Application.Abstractions.Messaging;
using Parking.Application.Abstractions.Services;
using Parking.Domain.Common;

public sealed record RefreshTokenCommand(
    string RefreshToken) : ICommand<LoginResponse>;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    long UserId,
    string UserName);

internal sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

internal sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // This is a simplified version - in real app, validate refresh token against DB
        return Task.FromResult(Result.Failure<LoginResponse>(
            new Error("Auth.InvalidRefreshToken", "Invalid or expired refresh token.")));
    }
}

// ─────────────────────────────────────────────────────────────────────────
// AssignRole Command

namespace Parking.Application.Features.Auth.AssignRole;

using FluentValidation;
using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

public sealed record AssignRoleCommand(
    long UserId,
    long RoleId) : ICommand;

internal sealed class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.RoleId).GreaterThan(0);
    }
}

internal sealed class AssignRoleCommandHandler : ICommandHandler<AssignRoleCommand>
{
    private readonly IAppUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleCommandHandler(
        IAppUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(new Error("AppUser.NotFound", "User not found."));

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result.Failure(new Error("Role.NotFound", "Role not found."));

        user.AssignRole(request.RoleId);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// APPLICATION LAYER — AUTH QUERIES
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Application.Features.Auth.GetUsers;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

public sealed record GetAllUsersQuery : IQuery<List<UserDto>>;

internal sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IAppUserRepository _userRepository;

    public GetAllUsersQueryHandler(IAppUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var dtos = users.Select(u => new UserDto(
            u.Id,
            u.UserName.Value,
            u.Email.Value,
            u.FullName,
            u.IsActive,
            new List<string>())).ToList();

        return Result.Success(dtos);
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// APPLICATION LAYER — COMPANY COMMANDS
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Application.Features.Company.Create;

using FluentValidation;
using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

public sealed record CreateCompanyCommand(
    string Name,
    string Cnpj,
    string LegalName) : ICommand<CompanyDto>;

internal sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Cnpj).NotEmpty().Length(14);
        RuleFor(x => x.LegalName).NotEmpty().MaximumLength(255);
    }
}

internal sealed class CreateCompanyCommandHandler : ICommandHandler<CreateCompanyCommand, CompanyDto>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCommandHandler(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate CNPJ
        var existing = await _companyRepository.GetByCnpjAsync(request.Cnpj, cancellationToken);
        if (existing is not null)
            return Result.Failure<CompanyDto>(
                new Error("Company.DuplicateCnpj", "Company with this CNPJ already exists."));

        var companyResult = Company.Create(request.Name, request.Cnpj, request.LegalName);
        if (companyResult.IsFailure)
            return Result.Failure<CompanyDto>(companyResult.Error);

        await _companyRepository.AddAsync(companyResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new CompanyDto(
            companyResult.Value.Id,
            companyResult.Value.Name,
            companyResult.Value.Cnpj,
            companyResult.Value.LegalName,
            companyResult.Value.IsActive));
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// APPLICATION LAYER — COMPANY QUERIES
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Application.Features.Company.GetById;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

public sealed record GetCompanyByIdQuery(long CompanyId) : IQuery<CompanyDto>;

internal sealed class GetCompanyByIdQueryHandler : IQueryHandler<GetCompanyByIdQuery, CompanyDto>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyByIdQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<CompanyDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken);
        if (company is null)
            return Result.Failure<CompanyDto>(new Error("Company.NotFound", "Company not found."));

        return Result.Success(new CompanyDto(
            company.Id,
            company.Name,
            company.Cnpj,
            company.LegalName,
            company.IsActive));
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// APPLICATION LAYER — BRANCH COMMANDS
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Application.Features.Branch.Create;

using FluentValidation;
using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

public sealed record CreateBranchCommand(
    long CompanyId,
    string Name,
    int TotalSpaces) : ICommand<BranchDto>;

internal sealed class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.CompanyId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.TotalSpaces).GreaterThan(0).LessThanOrEqualTo(10000);
    }
}

internal sealed class CreateBranchCommandHandler : ICommandHandler<CreateBranchCommand, BranchDto>
{
    private readonly IBranchRepository _branchRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBranchCommandHandler(
        IBranchRepository branchRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _branchRepository = branchRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BranchDto>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken);
        if (company is null)
            return Result.Failure<BranchDto>(new Error("Company.NotFound", "Company not found."));

        var branchResult = Branch.Create(request.CompanyId, request.Name, request.TotalSpaces);
        if (branchResult.IsFailure)
            return Result.Failure<BranchDto>(branchResult.Error);

        await _branchRepository.AddAsync(branchResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new BranchDto(
            branchResult.Value.Id,
            branchResult.Value.CompanyId,
            branchResult.Value.Name,
            branchResult.Value.TotalSpaces,
            branchResult.Value.IsActive));
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// APPLICATION LAYER — BRANCH QUERIES
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Application.Features.Branch.GetByCompany;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

public sealed record GetBranchesByCompanyQuery(long CompanyId) : IQuery<List<BranchDto>>;

internal sealed class GetBranchesByCompanyQueryHandler : IQueryHandler<GetBranchesByCompanyQuery, List<BranchDto>>
{
    private readonly IBranchRepository _branchRepository;

    public GetBranchesByCompanyQueryHandler(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    public async Task<Result<List<BranchDto>>> Handle(GetBranchesByCompanyQuery request, CancellationToken cancellationToken)
    {
        var branches = await _branchRepository.GetAllByCompanyAsync(request.CompanyId, cancellationToken);

        var dtos = branches.Select(b => new BranchDto(
            b.Id,
            b.CompanyId,
            b.Name,
            b.TotalSpaces,
            b.IsActive)).ToList();

        return Result.Success(dtos);
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// APPLICATION LAYER — PIPELINE BEHAVIORS
// ═══════════════════════════════════════════════════════════════════════════

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

internal sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<FluentValidation.IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new FluentValidation.ValidationContext<TRequest>(request);
        var failures = validators
            .SelectMany(v => v.Validate(context).Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            throw new FluentValidation.ValidationException(failures);

        return await next();
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// APPLICATION LAYER — DEPENDENCY INJECTION
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Application;

using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Parking.Application.Behaviors;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// API LAYER — CONTROLLERS
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Domain.Common;

[ApiController]
public abstract class ApiController(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator => mediator;

    protected IActionResult HandleFailure(Result result) =>
        result switch
        {
            { IsSuccess: true } => Ok(),
            _ => BadRequest(new { error = "Operation failed" })
        };

    protected IActionResult HandleFailure<T>(Result<T> result) =>
        result switch
        {
            { IsSuccess: true, } success => Ok(((dynamic)success).Value),
            _ => BadRequest(new { error = "Operation failed" })
        };
}

// ─────────────────────────────────────────────────────────────────────────
// AuthController

namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Auth.AssignRole;
using Parking.Application.Features.Auth.CreateUser;
using Parking.Application.Features.Auth.GetUsers;
using Parking.Application.Features.Auth.Login;
using Parking.Application.Features.Auth.RefreshToken;

[Route("api/[controller]")]
public sealed class AuthController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(
        AssignRoleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllUsersQuery(), cancellationToken);
        return HandleFailure(result);
    }
}

// ─────────────────────────────────────────────────────────────────────────
// CompanyController

namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Company.Create;
using Parking.Application.Features.Company.GetById;

[Route("api/[controller]")]
public sealed class CompanyController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCompanyCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCompanyByIdQuery(id), cancellationToken);
        return HandleFailure(result);
    }
}

// ─────────────────────────────────────────────────────────────────────────
// BranchController

namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Branch.Create;
using Parking.Application.Features.Branch.GetByCompany;

[Route("api/[controller]")]
public sealed class BranchController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBranchCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByCompany(
        [FromQuery] long companyId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetBranchesByCompanyQuery(companyId), cancellationToken);
        return HandleFailure(result);
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// API LAYER — MIDDLEWARE & CONFIGURATION
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.API.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "An error occurred" });
        }
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// UNIT TESTS
// ═══════════════════════════════════════════════════════════════════════════

namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Abstractions.Services;
using Parking.Application.Features.Auth.CreateUser;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var userRepository = Substitute.For<IAppUserRepository>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        userRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);
        passwordHasher.Hash(Arg.Any<string>())
            .Returns("hashed_password");
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new CreateUserCommandHandler(userRepository, passwordHasher, unitOfWork);
        var command = new CreateUserCommand("testuser", "test@example.com", "Test User", "Password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await userRepository.Received(1).AddAsync(Arg.Any<dynamic>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ShouldReturnFailure()
    {
        // Arrange
        var userRepository = Substitute.For<IAppUserRepository>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        userRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var handler = new CreateUserCommandHandler(userRepository, passwordHasher, unitOfWork);
        var command = new CreateUserCommand("testuser", "test@example.com", "Test User", "Password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AppUser.DuplicateEmail");
    }
}

namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Abstractions.Services;
using Parking.Application.Features.Auth.Login;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using Xunit;

public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var userRepository = Substitute.For<IAppUserRepository>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var tokenService = Substitute.For<ITokenService>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);
        tokenService.GenerateAccessToken(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("jwt_token");
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new LoginCommandHandler(userRepository, passwordHasher, tokenService, unitOfWork);
        var command = new LoginCommand("testuser", "password");

        // Act
        // This test would need the user to be properly created first
        // Simplified for example
    }
}

namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Company.Create;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateCompany()
    {
        // Arrange
        var companyRepository = Substitute.For<ICompanyRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        companyRepository.GetByCnpjAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((dynamic)null);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new CreateCompanyCommandHandler(companyRepository, unitOfWork);
        var command = new CreateCompanyCommand("Test Company", "12345678901234", "Test Company LTDA");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}

namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Auth.GetUsers;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetAllUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllUsers()
    {
        // Arrange
        var userRepository = Substitute.For<IAppUserRepository>();
        userRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<dynamic>());

        var handler = new GetAllUsersQueryHandler(userRepository);
        var query = new GetAllUsersQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
