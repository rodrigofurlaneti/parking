namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Company.GetById;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetCompanyByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingCompany_ShouldReturnCompanyDto()
    {
        // Arrange
        var companyRepository = Substitute.For<ICompanyRepository>();
        var company = Company.Create("Acme Estacionamentos", "12345678000190", "Acme Ltda").Value;

        companyRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(company);

        var handler = new GetCompanyByIdQueryHandler(companyRepository);

        var query = new GetCompanyByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Acme Estacionamentos");
        result.Value.Cnpj.Should().Be("12345678000190");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithUnknownCompany_ShouldFail()
    {
        // Arrange
        var companyRepository = Substitute.For<ICompanyRepository>();

        companyRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Company?)null);

        var handler = new GetCompanyByIdQueryHandler(companyRepository);

        var query = new GetCompanyByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Company.NotFound");
    }
}
