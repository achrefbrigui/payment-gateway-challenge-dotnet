using PaymentGateway.Api.Clients.Bank;
using Microsoft.AspNetCore.Mvc.Testing;
using PaymentGateway.Api.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace PaymentGateway.Api.Tests;

public class BankClientIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BankClientIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private static PostPaymentRequest CreateValidRequest()
    {
        var now = DateTime.UtcNow;
        var expiry = now.AddMonths(1);

        return new PostPaymentRequest
        {
            CardNumber = "1234567890123456",
            ExpiryDate = $"{expiry.Month:D2}/{expiry.Year}",
            Currency = "USD",
            Amount = 1000,
            Cvv = "123"
        };
    }

    [Fact]
    public async Task GivenAuthorizedCard_WhenAuthorizingPayment_ThenReturnsAuthorizedResponse()
    {
        // Arrange
        var bankClient = _factory.Services.GetRequiredService<IBankClient>();
        var request = CreateValidRequest();
        request.CardNumber = "1234567890123451";

        // Act
        var result = await bankClient.Authorize(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(result);
            Assert.True(result.Authorized);
            Assert.NotNull(result.AuthorizationCode);
        });
    }

    [Fact]
    public async Task GivenUnauthorizedCard_WhenAuthorizingPayment_ThenReturnsUnauthorizedResponse()
    {
        // Arrange
        var bankClient = _factory.Services.GetRequiredService<IBankClient>();

        var request = CreateValidRequest();
        request.CardNumber = "1234567890123452";

        // Act
        var result = await bankClient.Authorize(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(result);
            Assert.False(result.Authorized);
            Assert.True(string.IsNullOrEmpty(result.AuthorizationCode));
        });
    }

    [Fact]
    public async Task GivenUnavailableBank_WhenAuthorizingPayment_ThenThrowsHttpRequestException()
    {
        // Arrange
        var bankClient = _factory.Services.GetRequiredService<IBankClient>();

        var request = CreateValidRequest();
        request.CardNumber = "1234567890123450";

        // Act
        var act = () => bankClient.Authorize(request);

        // Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(act);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
    }
}