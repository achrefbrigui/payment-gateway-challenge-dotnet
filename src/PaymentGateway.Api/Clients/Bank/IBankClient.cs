using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Clients.Bank
{
    public interface IBankClient
    {
        public Task<BankAuthorizationResponse?> Authorize(PostPaymentRequest request);
    }
}
