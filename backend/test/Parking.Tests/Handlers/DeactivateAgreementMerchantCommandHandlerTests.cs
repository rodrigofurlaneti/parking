namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.AgreementMerchant.DeactivateAgreementMerchant;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class DeactivateAgreementMerchantCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidId_ShouldDeactivateAgreementMerchant()
    {
        // Arrange
        var repository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var merchant = Domain.Entities.AgreementMerchant.Create(1, "Original Company", 10m).Value;

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(merchant);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new DeactivateAgreementMerchantCommandHandler(repository, unitOfWork);
        var command = new DeactivateAgreementMerchantCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        merchant.IsActive.Should().BeFalse();

        await repository.Received(1).UpdateAsync(Arg.Any<AgreementMerchant>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentMerchant_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns((AgreementMerchant?)null);

        var handler = new DeactivateAgreementMerchantCommandHandler(repository, unitOfWork);
        var command = new DeactivateAgreementMerchantCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementMerchant.NotFound");
    }
}
