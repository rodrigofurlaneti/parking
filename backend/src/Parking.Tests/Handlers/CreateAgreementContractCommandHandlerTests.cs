namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.AgreementCustomerContract.CreateAgreementContract;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;
using DomainAgreementCustomerContract = Parking.Domain.Entities.AgreementCustomerContract;

public sealed class CreateAgreementContractCommandHandlerTests
{
    private static Customer CreateCustomer() =>
        Customer.Create(1, 2, "Jane Doe", "12345678900", "11999999999", "jane@doe.com").Value;

    private static AgreementMerchant CreateMerchant() =>
        AgreementMerchant.Create(1, "Restaurante Bom Sabor", 10m).Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateAgreementContract()
    {
        // Arrange
        var contractRepository = Substitute.For<IAgreementCustomerContractRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var agreementMerchantRepository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var customer = CreateCustomer();
        var merchant = CreateMerchant();

        customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        agreementMerchantRepository.GetByIdAsync(merchant.Id, Arg.Any<CancellationToken>()).Returns(merchant);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateAgreementContractCommandHandler(
            contractRepository, customerRepository, agreementMerchantRepository, unitOfWork);

        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddMonths(6);
        var command = new CreateAgreementContractCommand(customer.Id, merchant.Id, startDate, endDate);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CustomerId.Should().Be(customer.Id);
        result.Value.AgreementMerchantId.Should().Be(merchant.Id);
        result.Value.IsActive.Should().BeTrue();

        await contractRepository.Received(1).AddAsync(Arg.Any<DomainAgreementCustomerContract>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUnknownCustomer_ShouldFail()
    {
        // Arrange
        var contractRepository = Substitute.For<IAgreementCustomerContractRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var agreementMerchantRepository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        customerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var handler = new CreateAgreementContractCommandHandler(
            contractRepository, customerRepository, agreementMerchantRepository, unitOfWork);

        var command = new CreateAgreementContractCommand(1, 1, DateTime.UtcNow, DateTime.UtcNow.AddMonths(6));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.NotFound");
        await contractRepository.DidNotReceive().AddAsync(Arg.Any<DomainAgreementCustomerContract>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUnknownMerchant_ShouldFail()
    {
        // Arrange
        var contractRepository = Substitute.For<IAgreementCustomerContractRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var agreementMerchantRepository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var customer = CreateCustomer();
        customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        agreementMerchantRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((AgreementMerchant?)null);

        var handler = new CreateAgreementContractCommandHandler(
            contractRepository, customerRepository, agreementMerchantRepository, unitOfWork);

        var command = new CreateAgreementContractCommand(customer.Id, 1, DateTime.UtcNow, DateTime.UtcNow.AddMonths(6));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementMerchant.NotFound");
    }

    [Fact]
    public async Task Handle_WithEndDateBeforeStartDate_ShouldFail()
    {
        // Arrange
        var contractRepository = Substitute.For<IAgreementCustomerContractRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var agreementMerchantRepository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var customer = CreateCustomer();
        var merchant = CreateMerchant();

        customerRepository.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);
        agreementMerchantRepository.GetByIdAsync(merchant.Id, Arg.Any<CancellationToken>()).Returns(merchant);

        var handler = new CreateAgreementContractCommandHandler(
            contractRepository, customerRepository, agreementMerchantRepository, unitOfWork);

        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(-1);
        var command = new CreateAgreementContractCommand(customer.Id, merchant.Id, startDate, endDate);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementCustomerContract.InvalidPeriod");
    }
}
