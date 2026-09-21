using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Validators;

namespace PaymentGateway.Api.Tests;

public class PostPaymentRequestValidatorTests
{
    private readonly PostPaymentRequestValidator _validator = new();

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
    public void GivenValidPayment_WhenValidating_ThenIsValid()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    // Card number

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("1234567890123")]
    [InlineData("12345678901234567890")]
    [InlineData("123456789012345A")]
    [InlineData("1234-5678-9012-3456")]
    public void GivenInvalidCardNumber_WhenValidating_ThenIsInvalid(
        string? cardNumber)
    {
        // Arrange
        var request = CreateValidRequest();
        request.CardNumber = cardNumber;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("12345678901234")]
    [InlineData("1234567890123456")]
    [InlineData("1234567890123456789")]
    public void GivenValidCardNumber_WhenValidating_ThenHasNoCardNumberError(
        string cardNumber)
    {
        // Arrange
        var request = CreateValidRequest();
        request.CardNumber = cardNumber;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.DoesNotContain(
            result.Errors,
            error => error.PropertyName == nameof(PostPaymentRequest.CardNumber));
    }

    // Expiry date

    [Theory]
    [InlineData("01/2027")]
    [InlineData("09/2026")]
    [InlineData("12/2030")]
    public void GivenValidExpiryDate_WhenValidating_ThenHasNoExpiryDateError(
        string expiryDate)
    {
        // Arrange
        var request = CreateValidRequest();
        request.ExpiryDate = expiryDate;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.DoesNotContain(
            result.Errors,
            error => error.PropertyName == nameof(PostPaymentRequest.ExpiryDate));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GivenMissingExpiryDate_WhenValidating_ThenHasExpiryDateError(
        string? expiryDate)
    {
        // Arrange
        var request = CreateValidRequest();
        request.ExpiryDate = expiryDate;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(PostPaymentRequest.ExpiryDate));
    }

    [Theory]
    [InlineData("1/2027")]
    [InlineData("01/27")]
    [InlineData("2027/01")]
    [InlineData("01-2027")]
    [InlineData("January/2027")]
    public void GivenInvalidExpiryDateFormat_WhenValidating_ThenHasExpiryDateError(
        string expiryDate)
    {
        // Arrange
        var request = CreateValidRequest();
        request.ExpiryDate = expiryDate;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(PostPaymentRequest.ExpiryDate));
    }

    [Theory]
    [InlineData("00/2027")]
    [InlineData("13/2027")]
    public void GivenInvalidExpiryMonth_WhenValidating_ThenHasExpiryDateError(
        string expiryDate)
    {
        // Arrange
        var request = CreateValidRequest();
        request.ExpiryDate = expiryDate;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(PostPaymentRequest.ExpiryDate));
    }

    [Fact]
    public void GivenExpiredCard_WhenValidating_ThenHasExpiryDateError()
    {
        // Arrange
        var expiry = DateTime.UtcNow.AddMonths(-1);
        var request = CreateValidRequest();
        request.ExpiryDate = $"{expiry.Month:D2}/{expiry.Year}";

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(PostPaymentRequest.ExpiryDate));
    }

    [Fact]
    public void GivenCardExpiringThisMonth_WhenValidating_ThenHasNoExpiryDateError()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var request = CreateValidRequest();
        request.ExpiryDate = $"{now.Month:D2}/{now.Year}";

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.DoesNotContain(
            result.Errors,
            error => error.PropertyName == nameof(PostPaymentRequest.ExpiryDate));
    }

    [Fact]
    public void GivenCardExpiringNextMonth_WhenValidating_ThenHasNoExpiryDateError()
    {
        // Arrange
        var expiry = DateTime.UtcNow.AddMonths(1);
        var request = CreateValidRequest();
        request.ExpiryDate = $"{expiry.Month:D2}/{expiry.Year}";

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.DoesNotContain(
            result.Errors,
            error => error.PropertyName == nameof(PostPaymentRequest.ExpiryDate));
    }

    // Currency

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("BTC")]
    [InlineData("USDT")]
    [InlineData("TND")]
    public void GivenInvalidCurrency_WhenValidating_ThenIsInvalid(
        string? currency)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Currency = currency;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void GivenSupportedCurrency_WhenValidating_ThenHasNoCurrencyError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Currency = "USD";

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.DoesNotContain(
            result.Errors,
            error => error.PropertyName == nameof(PostPaymentRequest.Currency));
    }

    // Amount

    [Fact]
    public void GivenNegativeAmount_WhenValidating_ThenIsInvalid()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Amount = -1;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(10000)]
    public void GivenNonNegativeAmount_WhenValidating_ThenHasNoAmountError(int amount)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Amount = amount;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.DoesNotContain(
            result.Errors,
            error => error.PropertyName == nameof(PostPaymentRequest.Amount));
    }

    // CVV

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("12345")]
    [InlineData("abc")]
    [InlineData("12a")]
    public void GivenInvalidCvv_WhenValidating_ThenIsInvalid(string? cvv)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Cvv = cvv;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("4567")]
    public void GivenValidCvv_WhenValidating_ThenHasNoCvvError(string cvv)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Cvv = cvv;

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.DoesNotContain(
            result.Errors,
            error => error.PropertyName == nameof(PostPaymentRequest.Cvv));
    }
}
