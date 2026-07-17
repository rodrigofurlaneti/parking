namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.AgreementMerchant.GetAllByBranch;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetAllAgreementMerchantsByBranchQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithMerchantsInBranch_ShouldReturnList()
    {
        // Arrange
        var repository = Substitute.For<IAgreementMerchantRepository>();

        var merchant1 = Domain.Entities.AgreementMerchant.Create(1, "Company A", 10m).Value;
        var merchant2 = Domain.Entities.AgreementMerchant.Create(1, "Company B", 15m).Value;

        repository.GetAllByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgreementMerchant> { merchant1, merchant2 });

        var handler = new GetAllAgreementMerchantsByBranchQueryHandler(repository);
        var query = new GetAllAgreementMerchantsByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Select(x => x.CompanyName).Should().Contain(new[] { "Company A", "Company B" });
    }

    [Fact]
    public async Task Handle_WithNoMerchants_ShouldReturnEmptyList()
    {
        // Arrange
        var repository = Substitute.For<IAgreementMerchantRepository>();

        repository.GetAllByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgreementMerchant>());

        var handler = new GetAllAgreementMerchantsByBranchQueryHandler(repository);
        var query = new GetAllAgreementMerchantsByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
