namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Customer.GetAllByBranch;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetAllCustomersByBranchQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCustomers_ShouldReturnCustomerDtos()
    {
        // Arrange
        var customerRepository = Substitute.For<ICustomerRepository>();
        var customer1 = Customer.Create(1, 1, "Jane Doe", "12345678900", "11999999999", "jane@doe.com").Value;
        var customer2 = Customer.Create(1, 3, "John Smith", "98765432100", "11988888888", "john@smith.com").Value;

        customerRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<Customer> { customer1, customer2 });

        var handler = new GetAllCustomersByBranchQueryHandler(customerRepository);

        var query = new GetAllCustomersByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Select(c => c.Document).Should().Contain(new[] { "12345678900", "98765432100" });
    }

    [Fact]
    public async Task Handle_WithNoCustomers_ShouldReturnEmptyList()
    {
        // Arrange
        var customerRepository = Substitute.For<ICustomerRepository>();

        customerRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<Customer>());

        var handler = new GetAllCustomersByBranchQueryHandler(customerRepository);

        var query = new GetAllCustomersByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
