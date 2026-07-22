namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Branch.GetByCompany;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetBranchesByCompanyQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingBranches_ShouldReturnBranchDtos()
    {
        // Arrange
        var branchRepository = Substitute.For<IBranchRepository>();
        var branch1 = Branch.Create(1, "Filial Centro", "Rua A, 100", 50).Value;
        var branch2 = Branch.Create(1, "Filial Norte", "Rua B, 200", 30).Value;

        branchRepository.GetAllByCompanyAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<Branch> { branch1, branch2 });

        var handler = new GetBranchesByCompanyQueryHandler(branchRepository);

        var query = new GetBranchesByCompanyQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Select(b => b.Name).Should().Contain(new[] { "Filial Centro", "Filial Norte" });
    }

    [Fact]
    public async Task Handle_WithNoBranches_ShouldReturnEmptyList()
    {
        // Arrange
        var branchRepository = Substitute.For<IBranchRepository>();

        branchRepository.GetAllByCompanyAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<Branch>());

        var handler = new GetBranchesByCompanyQueryHandler(branchRepository);

        var query = new GetBranchesByCompanyQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
