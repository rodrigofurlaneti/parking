namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.VehicleEntry.RegisterVehicleEntry;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class RegisterVehicleEntryCommandHandlerTests
{
    private static Customer CreateCustomer(int customerType) =>
        Customer.Create(1, customerType, "John Doe", "12345678900", "11999999999", "john@doe.com").Value;

    private static MonthlyCustomerContract CreateMonthlyContract(long customerId, int maxVehicles, DateTime start, DateTime end) =>
        MonthlyCustomerContract.Create(customerId, 1, 200m, maxVehicles, start, end).Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldRegisterVehicleEntry()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var monthlyCustomerContractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var vehicleRepository = Substitute.For<IVehicleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var parkingSpace = Domain.Entities.ParkingSpace.Create(1, "A1", 1).Value;
        var customer = CreateCustomer(1); // Rotativo

        parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(parkingSpace);
        customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new RegisterVehicleEntryCommandHandler(
            vehicleEntryRepository, parkingSpaceRepository, customerRepository,
            monthlyCustomerContractRepository, vehicleRepository, unitOfWork);
        var command = new RegisterVehicleEntryCommand(1, 1, 1, "ABC-1234", "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LicensePlate.Should().Be("ABC-1234");
        result.Value.Status.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithUnavailableParkingSpace_ShouldFail()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var monthlyCustomerContractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var vehicleRepository = Substitute.For<IVehicleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var parkingSpace = Domain.Entities.ParkingSpace.Create(1, "A1", 1).Value;
        parkingSpace.MarkAsOccupied(); // Occupied

        parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(parkingSpace);

        var handler = new RegisterVehicleEntryCommandHandler(
            vehicleEntryRepository, parkingSpaceRepository, customerRepository,
            monthlyCustomerContractRepository, vehicleRepository, unitOfWork);
        var command = new RegisterVehicleEntryCommand(1, 1, 1, "ABC-1234", "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ParkingSpace.NotAvailable");
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ShouldFail()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var monthlyCustomerContractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var vehicleRepository = Substitute.For<IVehicleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var parkingSpace = Domain.Entities.ParkingSpace.Create(1, "A1", 1).Value;

        parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(parkingSpace);
        customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        var handler = new RegisterVehicleEntryCommandHandler(
            vehicleEntryRepository, parkingSpaceRepository, customerRepository,
            monthlyCustomerContractRepository, vehicleRepository, unitOfWork);
        var command = new RegisterVehicleEntryCommand(1, 1, 1, "ABC-1234", "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.NotFound");
    }

    [Fact]
    public async Task Handle_WithMonthlyCustomerWithoutActiveContract_ShouldFail()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var monthlyCustomerContractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var vehicleRepository = Substitute.For<IVehicleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var parkingSpace = Domain.Entities.ParkingSpace.Create(1, "A1", 1).Value;
        var customer = CreateCustomer(3); // Mensalista

        parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(parkingSpace);
        customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        monthlyCustomerContractRepository.GetActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<MonthlyCustomerContract>());

        var handler = new RegisterVehicleEntryCommandHandler(
            vehicleEntryRepository, parkingSpaceRepository, customerRepository,
            monthlyCustomerContractRepository, vehicleRepository, unitOfWork);
        var command = new RegisterVehicleEntryCommand(1, 1, 1, "ABC-1234", "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("MonthlyContract.Invalid");
    }

    [Fact]
    public async Task Handle_WithMonthlyCustomerAtVehicleLimitAndNewPlate_ShouldFail()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var monthlyCustomerContractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var vehicleRepository = Substitute.For<IVehicleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var parkingSpace = Domain.Entities.ParkingSpace.Create(1, "A1", 1).Value;
        var customer = CreateCustomer(3); // Mensalista
        var contract = CreateMonthlyContract(1, 1, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(10));
        var registeredVehicle = Vehicle.Create(1, "XYZ-9999", "Fiat Uno", "White").Value;

        parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(parkingSpace);
        customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        monthlyCustomerContractRepository.GetActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<MonthlyCustomerContract> { contract });
        vehicleRepository.CountActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(1);
        vehicleRepository.GetByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<Vehicle> { registeredVehicle });

        var handler = new RegisterVehicleEntryCommandHandler(
            vehicleEntryRepository, parkingSpaceRepository, customerRepository,
            monthlyCustomerContractRepository, vehicleRepository, unitOfWork);
        var command = new RegisterVehicleEntryCommand(1, 1, 1, "ABC-1234", "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("MonthlyContract.VehicleLimitExceeded");
    }

    [Fact]
    public async Task Handle_WithMonthlyCustomerWithinLimit_ShouldSucceed()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var monthlyCustomerContractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var vehicleRepository = Substitute.For<IVehicleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var parkingSpace = Domain.Entities.ParkingSpace.Create(1, "A1", 1).Value;
        var customer = CreateCustomer(3); // Mensalista
        var contract = CreateMonthlyContract(1, 3, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(10));

        parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(parkingSpace);
        customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        monthlyCustomerContractRepository.GetActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<MonthlyCustomerContract> { contract });
        vehicleRepository.CountActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(1);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new RegisterVehicleEntryCommandHandler(
            vehicleEntryRepository, parkingSpaceRepository, customerRepository,
            monthlyCustomerContractRepository, vehicleRepository, unitOfWork);
        var command = new RegisterVehicleEntryCommand(1, 1, 1, "ABC-1234", "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LicensePlate.Should().Be("ABC-1234");
    }
}
