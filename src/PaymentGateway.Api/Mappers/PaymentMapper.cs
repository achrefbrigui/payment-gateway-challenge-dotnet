using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Mappers
{
    public static class PaymentMapper
    {
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
