namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.CashRegister.CloseCashRegister;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CloseCashRegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithOpenRegister_ShouldCloseAndReturnDto()
    {
        // Arrange
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var register = CashRegister.Create(1, 1, 100m).Value;
        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(register);

        var handler = new CloseCashRegisterCommandHandler(registerRepository, unitOfWork);
        var command = new CloseCashRegisterCommand(1, 150m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ClosingBalance.Should().Be(150m);
        result.Value.Status.Should().Be(1);
        await registerRepository.Received(1).UpdateAsync(register, Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithRegisterNotFound_ShouldFail()
    {
        // Arrange
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((CashRegister?)null);

        var handler = new CloseCashRegisterCommandHandler(registerRepository, unitOfWork);
        var command = new CloseCashRegisterCommand(1, 150m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashRegister.NotFound");
    }

    [Fact]
    public async Task Handle_WithAlreadyClosedRegister_ShouldFail()
    {
        // Arrange
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var register = CashRegister.Create(1, 1, 100m).Value;
        register.Close(120m);
        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(register);

        var handler = new CloseCashRegisterCommandHandler(registerRepository, unitOfWork);
        var command = new CloseCashRegisterCommand(1, 150m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashRegister.AlreadyClosed");
    }
}
