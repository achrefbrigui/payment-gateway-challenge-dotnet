using System.Globalization;

using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Mappers
{
    public static class PaymentMapper
    {
        public static Payment ToDomain(this PostPaymentRequest request)
        {
            DateTime.TryParseExact(request.ExpiryDate, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var expiryDate);
            return new Payment(
            Guid.NewGuid(),
            request.CardNumber[^4..],
            expiryDate.Month,
            expiryDate.Year,
            request.Currency,
            request.Amount);
        }

        public static Payment ToDomain(this PostPaymentResponse request)
        {
            return new Payment(
            request.Id,
            request.CardNumberLastFour,
            request.ExpiryMonth,
            request.ExpiryYear,
            request.Currency,
            request.Amount);
        }

        public static GetPaymentResponse ToGetResponse(this Payment payment)
        {
            return new GetPaymentResponse
            {
                Id = payment.Id,
                Status = payment.Status,
                CardNumberLastFour = payment.CardNumberLastFour,
                ExpiryMonth = payment.ExpiryMonth,
                ExpiryYear = payment.ExpiryYear,
                Currency = payment.Currency,
                Amount = payment.Amount
            };
        }

        public static PostPaymentResponse ToPostResponse(this Payment payment)
        {
            return new PostPaymentResponse
            {
                Id = payment.Id,
                Status = payment.Status,
                CardNumberLastFour = payment.CardNumberLastFour,
                ExpiryMonth = payment.ExpiryMonth,
                ExpiryYear = payment.ExpiryYear,
                Currency = payment.Currency,
                Amount = payment.Amount
            };
        }
    }
}
