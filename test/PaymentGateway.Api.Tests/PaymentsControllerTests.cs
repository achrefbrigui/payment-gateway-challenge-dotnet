using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Mappers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Domain.Enums;

namespace PaymentGateway.Api.Tests;

/// <summary>
/// Payments controller intergation tests.
/// </summary>
public class PaymentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly Random _random = new();

    public PaymentsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
   
    private static PostPaymentRequest CreateValidRequest()
    {
        return new PostPaymentRequest
        {
            CardNumber = "1234567890123451",
            ExpiryDate = "12/2027",
            Currency = "GBP",
            Amount = 1000,
            Cvv = "123"
        };
    }

    #region GET

    [Fact]
    public async Task GivenPaymentExists_WhenRetrievingPayment_ThenReturnsPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(DateTime.UtcNow.Year, DateTime.UtcNow.Year + 10),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999).ToString(),
            Currency = "GBP"
        };

        var paymentsRepository = _factory.Services
             .GetRequiredService<IPaymentRepository>();
        paymentsRepository.Add(payment.ToDomain());

        // Act
        var response = await _client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(paymentResponse);
            Assert.Equal(payment.Id, paymentResponse?.Id);
            Assert.Equal(payment.ExpiryYear, paymentResponse?.ExpiryYear);
            Assert.Equal(payment.ExpiryMonth, paymentResponse?.ExpiryMonth);
            Assert.Equal(payment.Amount, paymentResponse?.Amount);
            Assert.Equal(payment.CardNumberLastFour, paymentResponse?.CardNumberLastFour);
            Assert.Equal(payment.Currency, paymentResponse?.Currency);
        });
    }

    [Fact]
    public async Task GivenPaymentDoesNotExist_WhenRetrievingPayment_ThenReturns404()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/Payments/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region POST

    [Fact]
    public async Task GivenValidPaymentRequest_WhenBankAuthorizesPayment_ThenReturnsAuthorizedPayment()
    {
        // Arrange
        var request = CreateValidRequest();
        request.CardNumber = "1234567890123451";

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/Payments",
            request);

        var paymentResponse = await response.Content
            .ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(paymentResponse);
            Assert.NotEqual(Guid.Empty, paymentResponse.Id);
            Assert.Equal(PaymentStatus.Authorized, paymentResponse.Status);
        });
    }

    [Fact]
    public async Task GivenValidPaymentRequest_WhenBankDeclinesPayment_ThenReturnsDeclinedPayment()
    {
        // Arrange
        var request = CreateValidRequest();
        request.CardNumber = "1234567890123452";

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/Payments",
            request);

        var paymentResponse = await response.Content
            .ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(paymentResponse);
            Assert.Equal(PaymentStatus.Declined, paymentResponse.Status);
        });
    }

    [Fact]
    public async Task GivenValidPaymentRequest_WhenBankIsUnavailable_ThenReturns503()
    {
        // Arrange
        var request = CreateValidRequest();
        request.CardNumber = "1234567890123450";

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/Payments",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    #endregion

}

