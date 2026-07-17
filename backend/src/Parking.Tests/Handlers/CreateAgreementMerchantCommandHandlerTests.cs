namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.AgreementMerchant.CreateAgreementMerchant;
using Parking.Domain.Repositories;
using Xunit;
using DomainAgreementMerchant = Parking.Domain.Entities.AgreementMerchant;

public sealed class CreateAgreementMerchantCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateAgreementMerchant()
    {
        // Arrange
        var agreementMerchantRepository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateAgreementMerchantCommandHandler(agreementMerchantRepository, unitOfWork);

        var command = new CreateAgreementMerchantCommand(1, "Restaurante Bom Sabor", 15m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.CompanyName.Should().Be("Restaurante Bom Sabor");
        result.Value.DiscountPercentage.Should().Be(15m);
        result.Value.IsActive.Should().BeTrue();

        await agreementMerchantRepository.Received(1).AddAsync(Arg.Any<DomainAgreementMerchant>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyCompanyName_ShouldFail()
    {
        // Arrange
        var agreementMerchantRepository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new CreateAgreementMerchantCommandHandler(agreementMerchantRepository, unitOfWork);

        var command = new CreateAgreementMerchantCommand(1, string.Empty, 15m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementMerchant.NameRequired");
        await agreementMerchantRepository.DidNotReceive().AddAsync(Arg.Any<DomainAgreementMerchant>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task Handle_WithInvalidDiscountPercentage_ShouldFail(decimal discount)
    {
        // Arrange
        var agreementMerchantRepository = Substitute.For<IAgreementMerchantRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new CreateAgreementMerchantCommandHandler(agreementMerchantRepository, unitOfWork);

        var command = new CreateAgreementMerchantCommand(1, "Restaurante Bom Sabor", discount);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementMerchant.InvalidDiscount");
    }
}
