namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Customer.CreateCustomer;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateCustomerCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithDuplicateDocument_ShouldFail()
    {
        // Arrange
        var customerRepository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        customerRepository.ExistsByDocumentAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var handler = new CreateCustomerCommandHandler(customerRepository, unitOfWork);
        var command = new CreateCustomerCommand(1, 1, "John Doe", "12345678900", "11999999999", "john@doe.com");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.DuplicateDocument");
        await customerRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.Customer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateCustomer()
    {
        // Arrange
        var customerRepository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        customerRepository.ExistsByDocumentAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new CreateCustomerCommandHandler(customerRepository, unitOfWork);
        var command = new CreateCustomerCommand(1, 1, "John Doe", "12345678900", "11999999999", "john@doe.com");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("John Doe");
        result.Value.Document.Should().Be("12345678900");
        result.Value.CustomerType.Should().Be(1);
        await customerRepository.Received(1).AddAsync(Arg.Any<Domain.Entities.Customer>(), Arg.Any<CancellationToken>());
    }
}
