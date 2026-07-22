namespace Parking.ArchTests;

using NetArchTest.Rules;
using Xunit;

public class ArchitectureTests
{
    private const string DomainNamespace = "Parking.Domain";
    private const string ApplicationNamespace = "Parking.Application";
    private const string InfrastructureNamespace = "Parking.Infrastructure";
    private const string ApiNamespace = "Parking.API";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        var result = Types.InAssembly(typeof(Parking.Domain.Common.Entity).Assembly)
            .Should()
            .NotHaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnInfrastructureOrApi()
    {
        var result = Types.InAssembly(typeof(Parking.Application.DependencyInjection).Assembly)
            .Should()
            .NotHaveDependencyOnAny(InfrastructureNamespace, ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
