namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ServiceCategory.GetAllByBranch;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetAllServiceCategoriesByBranchQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithCategoriesInBranch_ShouldReturnDtos()
    {
        // Arrange
        var serviceCategoryRepository = Substitute.For<IServiceCategoryRepository>();
        var category = ServiceCategory.Create(1, "Lavagem", "Servicos de lavagem").Value;

        serviceCategoryRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<ServiceCategory> { category });

        var handler = new GetAllServiceCategoriesByBranchQueryHandler(serviceCategoryRepository);
        var query = new GetAllServiceCategoriesByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Name.Should().Be("Lavagem");
    }

    [Fact]
    public async Task Handle_WithNoCategoriesInBranch_ShouldReturnEmptyList()
    {
        // Arrange
        var serviceCategoryRepository = Substitute.For<IServiceCategoryRepository>();
        serviceCategoryRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<ServiceCategory>());

        var handler = new GetAllServiceCategoriesByBranchQueryHandler(serviceCategoryRepository);
        var query = new GetAllServiceCategoriesByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
