namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Branch.Create;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateBranchCommandHandlerTests
{
    private static Company CreateCompany() =>
        Company.Create("Acme Estacionamentos", "12345678000190", "Acme Ltda").Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateBranch()
    {
        // Arrange
        var branchRepository = Substitute.For<IBranchRepository>();
        var companyRepository = Substitute.For<ICompanyRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var company = CreateCompany();
        companyRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(company);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateBranchCommandHandler(branchRepository, companyRepository, unitOfWork);

        var command = new CreateBranchCommand(1, "Filial Centro", "Rua A, 100", 50);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompanyId.Should().Be(1);
        result.Value.Name.Should().Be("Filial Centro");
        result.Value.TotalSpaces.Should().Be(50);
        result.Value.IsActive.Should().BeTrue();

        await branchRepository.Received(1).AddAsync(Arg.Any<Domain.Entities.Branch>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUnknownCompany_ShouldFail()
    {
        // Arrange
        var branchRepository = Substitute.For<IBranchRepository>();
        var companyRepository = Substitute.For<ICompanyRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        companyRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Company?)null);

        var handler = new CreateBranchCommandHandler(branchRepository, companyRepository, unitOfWork);

        var command = new CreateBranchCommand(1, "Filial Centro", "Rua A, 100", 50);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Company.NotFound");
        await branchRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.Branch>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldFail()
    {
        // Arrange
        var branchRepository = Substitute.For<IBranchRepository>();
        var companyRepository = Substitute.For<ICompanyRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var company = CreateCompany();
        companyRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(company);

        var handler = new CreateBranchCommandHandler(branchRepository, companyRepository, unitOfWork);

        var command = new CreateBranchCommand(1, string.Empty, "Rua A, 100", 50);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Branch.InvalidName");
    }

    [Fact]
    public async Task Handle_WithZeroTotalSpaces_ShouldFail()
    {
        // Arrange
        var branchRepository = Substitute.For<IBranchRepository>();
        var companyRepository = Substitute.For<ICompanyRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var company = CreateCompany();
        companyRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(company);

        var handler = new CreateBranchCommandHandler(branchRepository, companyRepository, unitOfWork);

        var command = new CreateBranchCommand(1, "Filial Centro", "Rua A, 100", 0);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Branch.InvalidTotalSpaces");
    }
}
