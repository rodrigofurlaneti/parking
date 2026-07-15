namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.CashRegister.OpenCashRegister;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class OpenCashRegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldOpenCashRegister()
    {
        // Arrange
        var cashRegisterRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        cashRegisterRepository.GetOpenByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns((CashRegister?)null);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new OpenCashRegisterCommandHandler(cashRegisterRepository, unitOfWork);
        var command = new OpenCashRegisterCommand(1, 1, 100m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.EmployeeId.Should().Be(1);
        result.Value.OpeningBalance.Should().Be(100m);
        result.Value.Status.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithAlreadyOpenRegister_ShouldFail()
    {
        // Arrange
        var cashRegisterRepository = Substitute.For<ICashRegisterRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var existingRegister = Substitute.For<Domain.Entities.CashRegister>();
        cashRegisterRepository.GetOpenByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(existingRegister);

        var handler = new OpenCashRegisterCommandHandler(cashRegisterRepository, unitOfWork);
        var command = new OpenCashRegisterCommand(1, 1, 100m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashRegister.AlreadyOpen");
    }
}
