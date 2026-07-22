namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.CashRegister.RecordCashMovement;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class RecordCashMovementCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidEntryMovement_ShouldRecordAndReturnDto()
    {
        // Arrange
        var movementRepository = Substitute.For<ICashMovementRepository>();
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var register = CashRegister.Create(1, 1, 100m).Value;
        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(register);

        var handler = new RecordCashMovementCommandHandler(movementRepository, registerRepository, unitOfWork);
        var command = new RecordCashMovementCommand(1, CashMovement.Entry, 50m, "Suprimento de caixa");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CashRegisterId.Should().Be(1);
        result.Value.Type.Should().Be(CashMovement.Entry);
        result.Value.Amount.Should().Be(50m);
        await movementRepository.Received(1).AddAsync(Arg.Any<CashMovement>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithRegisterNotFound_ShouldFail()
    {
        // Arrange
        var movementRepository = Substitute.For<ICashMovementRepository>();
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((CashRegister?)null);

        var handler = new RecordCashMovementCommandHandler(movementRepository, registerRepository, unitOfWork);
        var command = new RecordCashMovementCommand(1, CashMovement.Entry, 50m, "Suprimento de caixa");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashRegister.NotFound");
    }

    [Fact]
    public async Task Handle_WithClosedRegister_ShouldFail()
    {
        // Arrange
        var movementRepository = Substitute.For<ICashMovementRepository>();
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var register = CashRegister.Create(1, 1, 100m).Value;
        register.Close(100m);
        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(register);

        var handler = new RecordCashMovementCommandHandler(movementRepository, registerRepository, unitOfWork);
        var command = new RecordCashMovementCommand(1, CashMovement.Entry, 50m, "Suprimento de caixa");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashRegister.Closed");
    }

    [Fact]
    public async Task Handle_WithInvalidMovementType_ShouldFail()
    {
        // Arrange
        var movementRepository = Substitute.For<ICashMovementRepository>();
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var register = CashRegister.Create(1, 1, 100m).Value;
        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(register);

        var handler = new RecordCashMovementCommandHandler(movementRepository, registerRepository, unitOfWork);
        var command = new RecordCashMovementCommand(1, 99, 50m, "Tipo invalido");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashMovement.InvalidType");
    }

    [Fact]
    public async Task Handle_WithNonPositiveAmountForEntry_ShouldFail()
    {
        // Arrange
        var movementRepository = Substitute.For<ICashMovementRepository>();
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var register = CashRegister.Create(1, 1, 100m).Value;
        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(register);

        var handler = new RecordCashMovementCommandHandler(movementRepository, registerRepository, unitOfWork);
        var command = new RecordCashMovementCommand(1, CashMovement.Entry, 0m, "Valor invalido");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashMovement.InvalidAmount");
    }

    [Fact]
    public async Task Handle_WithZeroAmountForAdjustment_ShouldFail()
    {
        // Arrange
        var movementRepository = Substitute.For<ICashMovementRepository>();
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var register = CashRegister.Create(1, 1, 100m).Value;
        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(register);

        var handler = new RecordCashMovementCommandHandler(movementRepository, registerRepository, unitOfWork);
        var command = new RecordCashMovementCommand(1, CashMovement.Adjustment, 0m, "Ajuste invalido");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashMovement.InvalidAmount");
    }

    [Fact]
    public async Task Handle_WithNegativeAmountForAdjustment_ShouldSucceed()
    {
        // Arrange
        var movementRepository = Substitute.For<ICashMovementRepository>();
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var register = CashRegister.Create(1, 1, 100m).Value;
        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(register);

        var handler = new RecordCashMovementCommandHandler(movementRepository, registerRepository, unitOfWork);
        var command = new RecordCashMovementCommand(1, CashMovement.Adjustment, -25m, "Correcao para baixo");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(-25m);
    }
}
