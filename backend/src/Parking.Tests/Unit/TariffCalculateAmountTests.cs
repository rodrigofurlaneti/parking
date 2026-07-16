namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class TariffCalculateAmountTests
{
    private static Tariff CreateTariff(decimal firstHourRate, decimal additionalHourRate, decimal? dailyMaxRate = null) =>
        Tariff.Create(1, firstHourRate, additionalHourRate, dailyMaxRate).Value;

    [Fact]
    public void CalculateAmount_WithExactly60Minutes_ShouldChargeOnlyFirstHour()
    {
        var tariff = CreateTariff(10m, 5m);

        var amount = tariff.CalculateAmount(60);

        amount.Should().Be(10m);
    }

    [Fact]
    public void CalculateAmount_With61Minutes_ShouldChargeOneAdditionalHour()
    {
        var tariff = CreateTariff(10m, 5m);

        var amount = tariff.CalculateAmount(61);

        // 10 (primeira hora) + 1 hora adicional (fracao arredondada para cima) * 5 = 15
        amount.Should().Be(15m);
    }

    [Fact]
    public void CalculateAmount_With125Minutes_ShouldChargeTwoAdditionalHoursRoundingUp()
    {
        var tariff = CreateTariff(10m, 5m);

        var amount = tariff.CalculateAmount(125);

        // 125 - 60 = 65 minutos adicionais -> ceil(65/60) = 2 horas adicionais
        // 10 + 2 * 5 = 20
        amount.Should().Be(20m);
    }

    [Fact]
    public void CalculateAmount_WithoutDailyMaxRate_ShouldNotCapAmount()
    {
        var tariff = CreateTariff(10m, 5m);

        // 24h = 1440 min -> 1380 min adicionais -> 23 horas adicionais
        var amount = tariff.CalculateAmount(1440);

        amount.Should().Be(10m + (23 * 5m));
    }

    [Fact]
    public void CalculateAmount_WithDailyMaxRate_ShouldCapAtDailyMaxWhenExceeded()
    {
        var tariff = CreateTariff(10m, 5m, dailyMaxRate: 30m);

        var amount = tariff.CalculateAmount(300); // seria 10 + 4*5 = 30 sem teto, ainda igual ao teto
        var amountAboveCap = tariff.CalculateAmount(600); // seria 10 + 9*5 = 55, deve capar em 30

        amount.Should().Be(30m);
        amountAboveCap.Should().Be(30m);
    }

    [Fact]
    public void CalculateAmount_WithDailyMaxRate_ShouldNotCapWhenBelowMax()
    {
        var tariff = CreateTariff(10m, 5m, dailyMaxRate: 100m);

        var amount = tariff.CalculateAmount(61);

        amount.Should().Be(15m);
    }

    [Fact]
    public void Create_WithNegativeFirstHourRate_ShouldFail()
    {
        var result = Tariff.Create(1, -1m, 5m, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Tariff.InvalidFirstHourRate");
    }

    [Fact]
    public void Create_WithNegativeAdditionalHourRate_ShouldFail()
    {
        var result = Tariff.Create(1, 10m, -1m, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Tariff.InvalidAdditionalHourRate");
    }

    [Fact]
    public void Create_WithZeroDailyMaxRate_ShouldFail()
    {
        var result = Tariff.Create(1, 10m, 5m, 0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Tariff.InvalidDailyMaxRate");
    }
}
