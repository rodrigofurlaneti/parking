namespace Parking.Tests.Handlers;

using System.Reflection;
using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.VehicleEntry.RegisterVehicleExit;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class RegisterVehicleExitCommandHandlerTests
{
    private readonly IVehicleEntryRepository _vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
    private readonly IVehicleExitRepository _vehicleExitRepository = Substitute.For<IVehicleExitRepository>();
    private readonly IParkingSpaceRepository _parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
    private readonly ICustomerRepository _customerRepository = Substitute.For<ICustomerRepository>();
    private readonly ITariffRepository _tariffRepository = Substitute.For<ITariffRepository>();
    private readonly IAgreementCustomerContractRepository _agreementCustomerContractRepository = Substitute.For<IAgreementCustomerContractRepository>();
    private readonly IAgreementMerchantRepository _agreementMerchantRepository = Substitute.For<IAgreementMerchantRepository>();
    private readonly IMonthlyCustomerContractRepository _monthlyCustomerContractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private static void SetProperty(object obj, string propertyName, object? value)
    {
        var property = obj.GetType().GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
        property.SetValue(obj, value);
    }

    private static VehicleEntry CreateVehicleEntry(long branchId, long customerId)
    {
        var entry = VehicleEntry.Create(branchId, 1, customerId, "ABC-1234", "Honda Civic", "Black").Value;
        SetProperty(entry, nameof(VehicleEntry.EntryTime), DateTime.UtcNow.AddMinutes(-30));
        return entry;
    }

    private static Customer CreateCustomer(int customerType) =>
        Customer.Create(1, customerType, "John Doe", "12345678900", "11999999999", "john@doe.com").Value;

    private static Tariff CreateTariff(long branchId, decimal firstHourRate, decimal additionalHourRate) =>
        Tariff.Create(branchId, firstHourRate, additionalHourRate, null).Value;

    private RegisterVehicleExitCommandHandler CreateHandler() =>
        new(
            _vehicleEntryRepository,
            _vehicleExitRepository,
            _parkingSpaceRepository,
            _customerRepository,
            _tariffRepository,
            _agreementCustomerContractRepository,
            _agreementMerchantRepository,
            _monthlyCustomerContractRepository,
            _unitOfWork);

    [Fact]
    public async Task Handle_RotativoWithConfiguredTariff_ShouldCalculateAmountFromTariff()
    {
        var vehicleEntry = CreateVehicleEntry(1, 1);
        var customer = CreateCustomer(1);
        var tariff = CreateTariff(1, 10m, 5m);

        _vehicleEntryRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(vehicleEntry);
        _customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(customer);
        _tariffRepository.GetActiveByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(tariff);

        var handler = CreateHandler();
        var command = new RegisterVehicleExitCommand(1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(10m);
        result.Value.ParkingMode.Should().Be(1);
    }

    [Fact]
    public async Task Handle_RotativoWithoutConfiguredTariff_ShouldFail()
    {
        var vehicleEntry = CreateVehicleEntry(1, 1);
        var customer = CreateCustomer(1);

        _vehicleEntryRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(vehicleEntry);
        _customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(customer);
        _tariffRepository.GetActiveByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((Tariff?)null);

        var handler = CreateHandler();
        var command = new RegisterVehicleExitCommand(1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Tariff.NotConfigured");
    }

    [Fact]
    public async Task Handle_AgreementCustomerWithValidContract_ShouldApplyMerchantDiscount()
    {
        var vehicleEntry = CreateVehicleEntry(1, 1);
        var customer = CreateCustomer(2);
        var tariff = CreateTariff(1, 10m, 5m);
        var merchant = AgreementMerchant.Create(1, "ACME Corp", 20m).Value;
        var contract = AgreementCustomerContract.Create(
            customer.Id, merchant.Id, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(10)).Value;

        _vehicleEntryRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(vehicleEntry);
        _customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(customer);
        _tariffRepository.GetActiveByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(tariff);
        _agreementCustomerContractRepository.GetActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgreementCustomerContract> { contract });
        _agreementMerchantRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(merchant);

        var handler = CreateHandler();
        var command = new RegisterVehicleExitCommand(1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(8m);
        result.Value.ParkingMode.Should().Be(2);
    }

    [Fact]
    public async Task Handle_AgreementCustomerWithoutValidContract_ShouldFallBackToFullTariff()
    {
        var vehicleEntry = CreateVehicleEntry(1, 1);
        var customer = CreateCustomer(2);
        var tariff = CreateTariff(1, 10m, 5m);

        _vehicleEntryRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(vehicleEntry);
        _customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(customer);
        _tariffRepository.GetActiveByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(tariff);
        _agreementCustomerContractRepository.GetActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgreementCustomerContract>());

        var handler = CreateHandler();
        var command = new RegisterVehicleExitCommand(1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(10m);
    }

    [Fact]
    public async Task Handle_MonthlyCustomerWithValidContract_ShouldChargeZero()
    {
        var vehicleEntry = CreateVehicleEntry(1, 1);
        var customer = CreateCustomer(3);
        var contract = MonthlyCustomerContract.Create(
            customer.Id, 1, 200m, 1, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(10)).Value;

        _vehicleEntryRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(vehicleEntry);
        _customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(customer);
        _monthlyCustomerContractRepository.GetActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<MonthlyCustomerContract> { contract });

        var handler = CreateHandler();
        var command = new RegisterVehicleExitCommand(1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(0m);
        result.Value.ParkingMode.Should().Be(3);
        await _tariffRepository.DidNotReceive().GetActiveByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MonthlyCustomerWithoutValidContract_ShouldFallBackToFullTariff()
    {
        var vehicleEntry = CreateVehicleEntry(1, 1);
        var customer = CreateCustomer(3);
        var tariff = CreateTariff(1, 10m, 5m);

        _vehicleEntryRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(vehicleEntry);
        _customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(customer);
        _monthlyCustomerContractRepository.GetActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<MonthlyCustomerContract>());
        _tariffRepository.GetActiveByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(tariff);

        var handler = CreateHandler();
        var command = new RegisterVehicleExitCommand(1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(10m);
    }

    [Fact]
    public async Task Handle_WithNonExistentVehicleEntry_ShouldFail()
    {
        _vehicleEntryRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((VehicleEntry?)null);

        var handler = CreateHandler();
        var command = new RegisterVehicleExitCommand(1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VehicleEntry.NotFound");
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ShouldFail()
    {
        var vehicleEntry = CreateVehicleEntry(1, 1);

        _vehicleEntryRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(vehicleEntry);
        _customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var handler = CreateHandler();
        var command = new RegisterVehicleExitCommand(1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.NotFound");
    }
}
