namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Company.Create;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateCompany()
    {
        // Arrange
        var companyRepository = Substitute.For<ICompanyRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        companyRepository.GetByCnpjAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Company?)null);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new CreateCompanyCommandHandler(companyRepository, unitOfWork);
        var command = new CreateCompanyCommand("Test Company", "12345678901234", "Test Company LTDA");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
