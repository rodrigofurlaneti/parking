namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.VehicleEntry.RegisterVehicleEntryByPlate;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class RegisterVehicleEntryByPlateCommandHandlerTests
{
    private static Customer CreateCustomer(int customerType, string document = "12345678900") =>
        Customer.Create(1, customerType, "John Doe", document, "11999999999", "john@doe.com").Value;

    private static MonthlyCustomerContract CreateMonthlyContract(long customerId, int maxVehicles, DateTime start, DateTime end) =>
        MonthlyCustomerContract.Create(customerId, 1, 200m, maxVehicles, start, end).Value;

    private static (
        IVehicleEntryRepository vehicleEntryRepository,
        IParkingSpaceRepository parkingSpaceRepository,
        ICustomerRepository customerRepository,
        IVehicleRepository vehicleRepository,
        IMonthlyCustomerContractRepository monthlyCustomerContractRepository,
        IUnitOfWork unitOfWork,
        RegisterVehicleEntryByPlateCommandHandler handler) CreateSut()
    {
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var vehicleRepository = Substitute.For<IVehicleRepository>();
        var monthlyCustomerContractRepository = Substitute.For<IMonthlyCustomerContractRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new RegisterVehicleEntryByPlateCommandHandler(
            vehicleEntryRepository, parkingSpaceRepository, customerRepository,
            vehicleRepository, monthlyCustomerContractRepository, unitOfWork);

        return (vehicleEntryRepository, parkingSpaceRepository, customerRepository,
            vehicleRepository, monthlyCustomerContractRepository, unitOfWork, handler);
    }

    [Fact]
    public async Task Handle_WithKnownPlate_ShouldUseExistingCustomer_AndNotCreateNewOne()
    {
        var sut = CreateSut();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;
        var customer = CreateCustomer(1); // Rotativo
        var vehicle = Vehicle.Create(customer.Id, "ABC1D23", "Gol", "Prata").Value;

        sut.parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(parkingSpace);
        sut.vehicleEntryRepository.GetOpenByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((VehicleEntry?)null);
        sut.vehicleRepository.GetByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(vehicle);
        sut.customerRepository.GetByIdAsync(vehicle.CustomerId, Arg.Any<CancellationToken>()).Returns(customer);

        var command = new RegisterVehicleEntryByPlateCommand(1, 1, "abc1d23", null, null);

        var result = await sut.handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.LicensePlate.Should().Be("ABC1D23");
        result.Value.CustomerId.Should().Be(customer.Id);
        result.Value.IsNewCustomer.Should().BeFalse();
        await sut.customerRepository.DidNotReceive().AddAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
        await sut.unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUnknownPlate_ShouldAutoCreateAvulsoCustomerAndVehicle()
    {
        var sut = CreateSut();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;

        sut.parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(parkingSpace);
        sut.vehicleEntryRepository.GetOpenByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((VehicleEntry?)null);
        sut.vehicleRepository.GetByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Vehicle?)null);

        var command = new RegisterVehicleEntryByPlateCommand(1, 1, "XYZ9A88", "Onix", "Branco");

        var result = await sut.handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsNewCustomer.Should().BeTrue();
        result.Value.CustomerType.Should().Be(1); // Avulso/Rotativo
        result.Value.CustomerName.Should().Contain("XYZ9A88");
        await sut.customerRepository.Received(1).AddAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
        await sut.vehicleRepository.Received(1).AddAsync(Arg.Any<Vehicle>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithParkingSpaceNotFound_ShouldFail()
    {
        var sut = CreateSut();
        sut.parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((ParkingSpace?)null);

        var command = new RegisterVehicleEntryByPlateCommand(1, 1, "ABC1D23", null, null);
        var result = await sut.handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ParkingSpace.NotFound");
    }

    [Fact]
    public async Task Handle_WithParkingSpaceNotAvailable_ShouldFail()
    {
        var sut = CreateSut();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;
        parkingSpace.MarkAsOccupied();
        sut.parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(parkingSpace);

        var command = new RegisterVehicleEntryByPlateCommand(1, 1, "ABC1D23", null, null);
        var result = await sut.handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ParkingSpace.NotAvailable");
    }

    [Fact]
    public async Task Handle_WithPlateAlreadyParked_ShouldFail()
    {
        var sut = CreateSut();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;
        var openEntry = VehicleEntry.Create(1, 1, 1, "ABC1D23", "Gol", "Prata").Value;

        sut.parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(parkingSpace);
        sut.vehicleEntryRepository.GetOpenByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(openEntry);

        var command = new RegisterVehicleEntryByPlateCommand(1, 1, "ABC1D23", null, null);
        var result = await sut.handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VehicleEntry.AlreadyParked");
    }

    [Fact]
    public async Task Handle_WithOrphanedVehicle_ShouldFail_WhenCustomerNotFound()
    {
        var sut = CreateSut();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;
        var vehicle = Vehicle.Create(999, "ABC1D23", "Gol", "Prata").Value;

        sut.parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(parkingSpace);
        sut.vehicleEntryRepository.GetOpenByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((VehicleEntry?)null);
        sut.vehicleRepository.GetByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(vehicle);
        sut.customerRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var command = new RegisterVehicleEntryByPlateCommand(1, 1, "ABC1D23", null, null);
        var result = await sut.handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.NotFound");
        await sut.unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithMensalistaWithoutActiveContract_ShouldFail()
    {
        var sut = CreateSut();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;
        var customer = CreateCustomer(3); // Mensalista
        var vehicle = Vehicle.Create(customer.Id, "ABC1D23", "Gol", "Prata").Value;

        sut.parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(parkingSpace);
        sut.vehicleEntryRepository.GetOpenByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((VehicleEntry?)null);
        sut.vehicleRepository.GetByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(vehicle);
        sut.customerRepository.GetByIdAsync(vehicle.CustomerId, Arg.Any<CancellationToken>()).Returns(customer);
        sut.monthlyCustomerContractRepository.GetActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<MonthlyCustomerContract>());

        var command = new RegisterVehicleEntryByPlateCommand(1, 1, "ABC1D23", null, null);
        var result = await sut.handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("MonthlyContract.Invalid");
    }

    [Fact]
    public async Task Handle_WithMensalistaAtVehicleLimitAndUnregisteredPlate_ShouldFail()
    {
        var sut = CreateSut();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;
        var customer = CreateCustomer(3); // Mensalista
        var vehicle = Vehicle.Create(customer.Id, "ABC1D23", "Gol", "Prata").Value;
        var contract = CreateMonthlyContract(customer.Id, 1, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(10));
        var otherRegisteredVehicle = Vehicle.Create(customer.Id, "XYZ9999", "Fiat Uno", "Branco").Value;

        sut.parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(parkingSpace);
        sut.vehicleEntryRepository.GetOpenByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((VehicleEntry?)null);
        sut.vehicleRepository.GetByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(vehicle);
        sut.customerRepository.GetByIdAsync(vehicle.CustomerId, Arg.Any<CancellationToken>()).Returns(customer);
        sut.monthlyCustomerContractRepository.GetActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<MonthlyCustomerContract> { contract });
        sut.vehicleRepository.CountActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(1);
        sut.vehicleRepository.GetByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<Vehicle> { otherRegisteredVehicle });

        var command = new RegisterVehicleEntryByPlateCommand(1, 1, "ABC1D23", null, null);
        var result = await sut.handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("MonthlyContract.VehicleLimitExceeded");
    }

    [Fact]
    public async Task Handle_WithMensalistaAtVehicleLimitButPlateAlreadyRegistered_ShouldSucceed()
    {
        var sut = CreateSut();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;
        var customer = CreateCustomer(3); // Mensalista
        var vehicle = Vehicle.Create(customer.Id, "ABC1D23", "Gol", "Prata").Value;
        var contract = CreateMonthlyContract(customer.Id, 1, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(10));

        sut.parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(parkingSpace);
        sut.vehicleEntryRepository.GetOpenByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((VehicleEntry?)null);
        sut.vehicleRepository.GetByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(vehicle);
        sut.customerRepository.GetByIdAsync(vehicle.CustomerId, Arg.Any<CancellationToken>()).Returns(customer);
        sut.monthlyCustomerContractRepository.GetActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<MonthlyCustomerContract> { contract });
        sut.vehicleRepository.CountActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(1);
        sut.vehicleRepository.GetByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<Vehicle> { vehicle }); // same plate already registered

        var command = new RegisterVehicleEntryByPlateCommand(1, 1, "ABC1D23", null, null);
        var result = await sut.handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithMensalistaWithinLimit_ShouldSucceed()
    {
        var sut = CreateSut();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;
        var customer = CreateCustomer(3); // Mensalista
        var vehicle = Vehicle.Create(customer.Id, "ABC1D23", "Gol", "Prata").Value;
        var contract = CreateMonthlyContract(customer.Id, 3, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(10));

        sut.parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(parkingSpace);
        sut.vehicleEntryRepository.GetOpenByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((VehicleEntry?)null);
        sut.vehicleRepository.GetByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(vehicle);
        sut.customerRepository.GetByIdAsync(vehicle.CustomerId, Arg.Any<CancellationToken>()).Returns(customer);
        sut.monthlyCustomerContractRepository.GetActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<MonthlyCustomerContract> { contract });
        sut.vehicleRepository.CountActiveByCustomerAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(1);

        var command = new RegisterVehicleEntryByPlateCommand(1, 1, "ABC1D23", null, null);
        var result = await sut.handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.CustomerType.Should().Be(3);
    }
}
