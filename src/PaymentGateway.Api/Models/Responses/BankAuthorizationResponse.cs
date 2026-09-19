namespace PaymentGateway.Api.Models.Responses
{
    public class BankAuthorizationResponse
    {
        public bool Authorized { get; set; }
        public string? AuthorizationCode { get; set; }
    }
}
