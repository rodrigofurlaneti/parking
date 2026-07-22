namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Vehicle.CreateVehicle;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateVehicleCommandHandlerTests
{
    private static Customer CreateCustomer() =>
        Customer.Create(1, 1, "John Doe", "12345678900", "11999999999", "john@doe.com").Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateVehicle()
    {
        // Arrange
        var vehicleRepository = Substitute.For<IVehicleRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var customer = CreateCustomer();

        customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        vehicleRepository.GetByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Vehicle?)null);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new CreateVehicleCommandHandler(vehicleRepository, customerRepository, unitOfWork);
        var command = new CreateVehicleCommand(1, "ABC-1234", "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LicensePlate.Should().Be("ABC-1234");
        result.Value.CustomerId.Should().Be(1);
        result.Value.IsActive.Should().BeTrue();

        await vehicleRepository.Received(1).AddAsync(Arg.Any<Vehicle>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ShouldFail()
    {
        // Arrange
        var vehicleRepository = Substitute.For<IVehicleRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        var handler = new CreateVehicleCommandHandler(vehicleRepository, customerRepository, unitOfWork);
        var command = new CreateVehicleCommand(1, "ABC-1234", "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.NotFound");

        await vehicleRepository.DidNotReceive().AddAsync(Arg.Any<Vehicle>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateLicensePlate_ShouldFail()
    {
        // Arrange
        var vehicleRepository = Substitute.For<IVehicleRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var customer = CreateCustomer();
        var existingVehicle = Vehicle.Create(1, "ABC-1234", "Fiat Uno", "White").Value;

        customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        vehicleRepository.GetByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(existingVehicle);

        var handler = new CreateVehicleCommandHandler(vehicleRepository, customerRepository, unitOfWork);
        var command = new CreateVehicleCommand(1, "abc-1234", "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Vehicle.DuplicatePlate");

        await vehicleRepository.DidNotReceive().AddAsync(Arg.Any<Vehicle>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyLicensePlate_ShouldFail()
    {
        // Arrange
        var vehicleRepository = Substitute.For<IVehicleRepository>();
        var customerRepository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var customer = CreateCustomer();

        customerRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        vehicleRepository.GetByLicensePlateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Vehicle?)null);

        var handler = new CreateVehicleCommandHandler(vehicleRepository, customerRepository, unitOfWork);
        var command = new CreateVehicleCommand(1, string.Empty, "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Vehicle.InvalidPlate");
    }
}
