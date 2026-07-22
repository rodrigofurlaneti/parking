namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Customer.GetCustomerByDocument;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetCustomerByDocumentQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingDocument_ShouldReturnCustomerDto()
    {
        // Arrange
        var customerRepository = Substitute.For<ICustomerRepository>();
        var customer = Customer.Create(1, 1, "Jane Doe", "12345678900", "11999999999", "jane@doe.com").Value;

        customerRepository.GetByDocumentAsync("12345678900", Arg.Any<CancellationToken>()).Returns(customer);

        var handler = new GetCustomerByDocumentQueryHandler(customerRepository);

        var query = new GetCustomerByDocumentQuery("12345678900");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Document.Should().Be("12345678900");
        result.Value.Name.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task Handle_WithUnknownDocument_ShouldFail()
    {
        // Arrange
        var customerRepository = Substitute.For<ICustomerRepository>();

        customerRepository.GetByDocumentAsync("00000000000", Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var handler = new GetCustomerByDocumentQueryHandler(customerRepository);

        var query = new GetCustomerByDocumentQuery("00000000000");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.NotFound");
    }
}
