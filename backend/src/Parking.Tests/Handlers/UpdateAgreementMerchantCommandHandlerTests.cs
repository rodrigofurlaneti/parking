namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.AgreementMerchant.UpdateAgreementMerchant;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class UpdateAgreementMerchantCommandHandlerTests
{
    private static AgreementMerchant CreateMerchant() =>
        Domain.Entities.AgreementMerchant.Create(1, "Original Company", 10m).Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateAgreementMerchant()
    {
        // Arrange
        var repository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var merchant = CreateMerchant();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(merchant);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new UpdateAgreementMerchantCommandHandler(repository, unitOfWork);
        var command = new UpdateAgreementMerchantCommand(1, "Updated Company", 20m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompanyName.Should().Be("Updated Company");
        result.Value.DiscountPercentage.Should().Be(20m);

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

        var handler = new UpdateAgreementMerchantCommandHandler(repository, unitOfWork);
        var command = new UpdateAgreementMerchantCommand(1, "Updated Company", 20m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementMerchant.NotFound");
    }

    [Fact]
    public async Task Handle_WithEmptyCompanyName_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var merchant = CreateMerchant();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(merchant);

        var handler = new UpdateAgreementMerchantCommandHandler(repository, unitOfWork);
        var command = new UpdateAgreementMerchantCommand(1, string.Empty, 20m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementMerchant.NameRequired");
    }

    [Fact]
    public async Task Handle_WithInvalidDiscountPercentage_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var merchant = CreateMerchant();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(merchant);

        var handler = new UpdateAgreementMerchantCommandHandler(repository, unitOfWork);
        var command = new UpdateAgreementMerchantCommand(1, "Updated Company", 150m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementMerchant.InvalidDiscount");
    }
}
