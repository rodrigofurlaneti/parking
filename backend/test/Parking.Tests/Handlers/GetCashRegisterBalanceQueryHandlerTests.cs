namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.CashRegister.GetCashRegisterBalance;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetCashRegisterBalanceQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingRegister_ShouldReturnCalculatedBalance()
    {
        // Arrange
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var movementRepository = Substitute.For<ICashMovementRepository>();

        var register = CashRegister.Create(1, 1, 100m).Value;
        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(register);
        movementRepository.GetTotalByRegisterAsync(1, Arg.Any<CancellationToken>()).Returns(50m);

        var handler = new GetCashRegisterBalanceQueryHandler(registerRepository, movementRepository);
        var query = new GetCashRegisterBalanceQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OpeningBalance.Should().Be(100m);
        result.Value.TotalMovements.Should().Be(50m);
        result.Value.CalculatedBalance.Should().Be(150m);
    }

    [Fact]
    public async Task Handle_WithRegisterNotFound_ShouldFail()
    {
        // Arrange
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var movementRepository = Substitute.For<ICashMovementRepository>();

        registerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((CashRegister?)null);

        var handler = new GetCashRegisterBalanceQueryHandler(registerRepository, movementRepository);
        var query = new GetCashRegisterBalanceQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashRegister.NotFound");
    }
}
