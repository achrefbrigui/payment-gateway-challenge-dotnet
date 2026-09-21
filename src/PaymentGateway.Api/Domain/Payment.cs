using PaymentGateway.Api.Domain.Enums;

namespace PaymentGateway.Api.Domain
{
    public class Payment
    {
        public Guid Id { get; set; }
        public PaymentStatus Status { get; set; }
        public string CardNumberLastFour { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Currency { get; set; }
        public int Amount { get; set; }
        public string? AuthorizationCode { get; set; }

        public Payment(
            Guid id,
            string cardNumberLastFour,
            int expiryMonth,
            int expiryYear,
            string currency,
            int amount)
        {
            Id = id;
            CardNumberLastFour = cardNumberLastFour;
            ExpiryMonth = expiryMonth;
            ExpiryYear = expiryYear;
            Currency = currency;
            Amount = amount;
            Status = PaymentStatus.Rejected;
        }

        public void Authorize(string authorizationCode)
        {
            Status = PaymentStatus.Authorized;
            AuthorizationCode = AuthorizationCode;
        }

        public void Decline()
        {
            Status = PaymentStatus.Declined;
            AuthorizationCode = null;
        }

        public void Reject()
        {
            Status = PaymentStatus.Rejected;
            AuthorizationCode = null;
        }
    }
}
