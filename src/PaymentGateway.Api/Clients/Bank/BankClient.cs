
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Clients.Bank
{
    public class BankClient : IBankClient
    {
        private readonly HttpClient _httpclient;

        public BankClient(HttpClient httpclient)
        {
            _httpclient = httpclient;
        }
        public async Task<BankAuthorizationResponse?> Authorize(PostPaymentRequest request)
        {
            var response = await _httpclient.PostAsJsonAsync("payments", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<BankAuthorizationResponse>();

            return result;
        }
    }
}