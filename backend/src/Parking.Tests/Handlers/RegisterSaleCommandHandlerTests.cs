namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Sale.RegisterSale;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class RegisterSaleCommandHandlerTests
{
    private static VehicleExit CreateVehicleExit(decimal totalAmount) =>
        VehicleExit.Create(1, 60, totalAmount, 1).Value;

    private static CashRegister CreateOpenCashRegister() =>
        CashRegister.Create(1, 1, 100m).Value;

    [Fact]
    public async Task Handle_WithPaymentMismatch_ShouldFail()
    {
        // Arrange
        var vehicleExitRepository = Substitute.For<IVehicleExitRepository>();
        var saleRepository = Substitute.For<ISaleRepository>();
        var salePaymentRepository = Substitute.For<ISalePaymentRepository>();
        var cashRegisterRepository = Substitute.For<ICashRegisterRepository>();
        var cashMovementRepository = Substitute.For<ICashMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var vehicleExit = CreateVehicleExit(50m);
        var cashRegister = CreateOpenCashRegister();

        vehicleExitRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(vehicleExit);
        saleRepository.ExistsActiveByVehicleExitAsync(1, Arg.Any<CancellationToken>()).Returns(false);
        cashRegisterRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(cashRegister);

        var handler = new RegisterSaleCommandHandler(
            vehicleExitRepository,
            saleRepository,
            salePaymentRepository,
            cashRegisterRepository,
            cashMovementRepository,
            unitOfWork);

        var command = new RegisterSaleCommand(1, 1, 1, new List<PaymentInput> { new(1, 30m) });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Sale.PaymentMismatch");
    }

    [Fact]
    public async Task Handle_WithExistingActiveSale_ShouldFail()
    {
        // Arrange
        var vehicleExitRepository = Substitute.For<IVehicleExitRepository>();
        var saleRepository = Substitute.For<ISaleRepository>();
        var salePaymentRepository = Substitute.For<ISalePaymentRepository>();
        var cashRegisterRepository = Substitute.For<ICashRegisterRepository>();
        var cashMovementRepository = Substitute.For<ICashMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var vehicleExit = CreateVehicleExit(50m);

        vehicleExitRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(vehicleExit);
        saleRepository.ExistsActiveByVehicleExitAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        var handler = new RegisterSaleCommandHandler(
            vehicleExitRepository,
            saleRepository,
            salePaymentRepository,
            cashRegisterRepository,
            cashMovementRepository,
            unitOfWork);

        var command = new RegisterSaleCommand(1, 1, 1, new List<PaymentInput> { new(1, 50m) });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Sale.AlreadyExists");
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldRegisterSaleWithPaymentsAndCashMovement()
    {
        // Arrange
        var vehicleExitRepository = Substitute.For<IVehicleExitRepository>();
        var saleRepository = Substitute.For<ISaleRepository>();
        var salePaymentRepository = Substitute.For<ISalePaymentRepository>();
        var cashRegisterRepository = Substitute.For<ICashRegisterRepository>();
        var cashMovementRepository = Substitute.For<ICashMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var vehicleExit = CreateVehicleExit(80m);
        var cashRegister = CreateOpenCashRegister();

        vehicleExitRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(vehicleExit);
        saleRepository.ExistsActiveByVehicleExitAsync(10, Arg.Any<CancellationToken>()).Returns(false);
        cashRegisterRepository.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(cashRegister);
        saleRepository.GetNextSaleNumberAsync(2, Arg.Any<CancellationToken>()).Returns(1L);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new RegisterSaleCommandHandler(
            vehicleExitRepository,
            saleRepository,
            salePaymentRepository,
            cashRegisterRepository,
            cashMovementRepository,
            unitOfWork);

        var command = new RegisterSaleCommand(
            2,
            10,
            5,
            new List<PaymentInput> { new(1, 50m), new(4, 30m) });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(2);
        result.Value.VehicleExitId.Should().Be(10);
        result.Value.SaleNumber.Should().Be(1);
        result.Value.TotalAmount.Should().Be(80m);
        result.Value.IsActive.Should().BeTrue();
        result.Value.Payments.Should().HaveCount(2);

        await saleRepository.Received(1).AddAsync(Arg.Any<Domain.Entities.Sale>(), Arg.Any<CancellationToken>());
        await salePaymentRepository.Received(2).AddAsync(Arg.Any<SalePayment>(), Arg.Any<CancellationToken>());
        await cashMovementRepository.Received(1).AddAsync(
            Arg.Is<CashMovement>(m => m.CashRegisterId == 5 && m.Amount == 80m && m.Type == 1),
            Arg.Any<CancellationToken>());
        await unitOfWork.Received(2).CommitAsync(Arg.Any<CancellationToken>());
    }
}
