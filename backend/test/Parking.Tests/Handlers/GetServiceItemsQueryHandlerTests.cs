namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ServiceItem.GetAll;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetServiceItemsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithItemsInCategory_ShouldReturnDtos()
    {
        // Arrange
        var serviceItemRepository = Substitute.For<IServiceItemRepository>();
        var item = ServiceItem.Create(1, "Lavagem Simples", "Lavagem externa", 30, 25m).Value;

        serviceItemRepository.GetAllByCategoryAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<ServiceItem> { item });

        var handler = new GetServiceItemsQueryHandler(serviceItemRepository);
        var query = new GetServiceItemsQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Name.Should().Be("Lavagem Simples");
    }

    [Fact]
    public async Task Handle_WithNoItemsInCategory_ShouldReturnEmptyList()
    {
        // Arrange
        var serviceItemRepository = Substitute.For<IServiceItemRepository>();
        serviceItemRepository.GetAllByCategoryAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<ServiceItem>());

        var handler = new GetServiceItemsQueryHandler(serviceItemRepository);
        var query = new GetServiceItemsQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
