namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.CashRegister.GetOpenByBranch;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetOpenCashRegisterByBranchQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithOpenRegisterInBranch_ShouldReturnDto()
    {
        // Arrange
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        var register = CashRegister.Create(1, 1, 100m).Value;
        registerRepository.GetOpenByBranchAsync(1, Arg.Any<CancellationToken>()).Returns(register);

        var handler = new GetOpenCashRegisterByBranchQueryHandler(registerRepository);
        var query = new GetOpenCashRegisterByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.BranchId.Should().Be(1);
        result.Value.OpeningBalance.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_WithNoOpenRegister_ShouldReturnSuccessWithNull()
    {
        // Arrange
        var registerRepository = Substitute.For<ICashRegisterRepository>();
        registerRepository.GetOpenByBranchAsync(1, Arg.Any<CancellationToken>()).Returns((CashRegister?)null);

        var handler = new GetOpenCashRegisterByBranchQueryHandler(registerRepository);
        var query = new GetOpenCashRegisterByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }
}
