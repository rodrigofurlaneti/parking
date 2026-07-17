namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.MonthlyCustomerContract.CreateMonthlyContract;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;
using DomainMonthlyCustomerContract = Parking.Domain.Entities.MonthlyCustomerContract;

public sealed class CreateMonthlyContractCommandHandlerTests
{
    private static Customer CreateCustomer() =>
        Customer.Create(1, 3, "Jane Doe", "12345678900", "11999999999", "jane@doe.com").Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateMonthlyContract()
    {
        // Arrange
        var contractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var customer = CreateCustomer();
        customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateMonthlyContractCommandHandler(contractRepository, customerRepository, unitOfWork);

        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddMonths(1);
        var command = new CreateMonthlyContractCommand(customer.Id, 1, 200m, 2, startDate, endDate);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CustomerId.Should().Be(customer.Id);
        result.Value.BranchId.Should().Be(1);
        result.Value.MonthlyFee.Should().Be(200m);
        result.Value.MaxVehicles.Should().Be(2);
        result.Value.IsActive.Should().BeTrue();

        await contractRepository.Received(1).AddAsync(Arg.Any<DomainMonthlyCustomerContract>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUnknownCustomer_ShouldFail()
    {
        // Arrange
        var contractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        customerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var handler = new CreateMonthlyContractCommandHandler(contractRepository, customerRepository, unitOfWork);

        var command = new CreateMonthlyContractCommand(1, 1, 200m, 2, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.NotFound");
        await contractRepository.DidNotReceive().AddAsync(Arg.Any<DomainMonthlyCustomerContract>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithZeroMonthlyFee_ShouldFail()
    {
        // Arrange
        var contractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var customer = CreateCustomer();
        customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var handler = new CreateMonthlyContractCommandHandler(contractRepository, customerRepository, unitOfWork);

        var command = new CreateMonthlyContractCommand(customer.Id, 1, 0m, 2, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("MonthlyCustomerContract.InvalidFee");
    }

    [Fact]
    public async Task Handle_WithZeroMaxVehicles_ShouldFail()
    {
        // Arrange
        var contractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var customer = CreateCustomer();
        customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var handler = new CreateMonthlyContractCommandHandler(contractRepository, customerRepository, unitOfWork);

        var command = new CreateMonthlyContractCommand(customer.Id, 1, 200m, 0, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("MonthlyCustomerContract.InvalidMaxVehicles");
    }

    [Fact]
    public async Task Handle_WithEndDateBeforeStartDate_ShouldFail()
    {
        // Arrange
        var contractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var customer = CreateCustomer();
        customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var handler = new CreateMonthlyContractCommandHandler(contractRepository, customerRepository, unitOfWork);

        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(-1);
        var command = new CreateMonthlyContractCommand(customer.Id, 1, 200m, 2, startDate, endDate);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("MonthlyCustomerContract.InvalidPeriod");
    }
}
