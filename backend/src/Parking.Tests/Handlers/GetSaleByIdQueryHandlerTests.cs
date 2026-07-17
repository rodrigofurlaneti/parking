namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Sale.GetSaleById;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetSaleByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingSale_ShouldReturnDtoWithPayments()
    {
        // Arrange
        var saleRepository = Substitute.For<ISaleRepository>();
        var salePaymentRepository = Substitute.For<ISalePaymentRepository>();

        var sale = Sale.Create(1, 1, 1, 1001, 50m).Value;
        var payment = SalePayment.Create(sale.Id, 1, 50m).Value;

        saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        salePaymentRepository.GetBySaleAsync(sale.Id, Arg.Any<CancellationToken>())
            .Returns(new List<SalePayment> { payment });

        var handler = new GetSaleByIdQueryHandler(saleRepository, salePaymentRepository);
        var query = new GetSaleByIdQuery(sale.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SaleNumber.Should().Be(1001);
        result.Value.TotalAmount.Should().Be(50m);
        result.Value.Payments.Should().HaveCount(1);
        result.Value.Payments.First().Amount.Should().Be(50m);
    }

    [Fact]
    public async Task Handle_WithSaleNotFound_ShouldFail()
    {
        // Arrange
        var saleRepository = Substitute.For<ISaleRepository>();
        var salePaymentRepository = Substitute.For<ISalePaymentRepository>();

        saleRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var handler = new GetSaleByIdQueryHandler(saleRepository, salePaymentRepository);
        var query = new GetSaleByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Sale.NotFound");
    }
}
