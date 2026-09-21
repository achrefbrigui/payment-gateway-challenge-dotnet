using PaymentGateway.Api.Clients.Bank;
using PaymentGateway.Api.Mappers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBankClient _bankClient;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IBankClient bankClient, IPaymentRepository paymentRepository)
        {
            _bankClient = bankClient;
            _paymentRepository = paymentRepository;
        }

        public async Task<PostPaymentResponse> ProcessPayment(PostPaymentRequest request)
        {
            var payment = request.ToDomain();

            var bankResponse = await _bankClient.Authorize(request);
            if (bankResponse?.Authorized == true && !string.IsNullOrEmpty(bankResponse?.AuthorizationCode))
            {
                payment.Authorize(bankResponse.AuthorizationCode);
            }
            else
            {
                payment.Decline();
            }

            _paymentRepository.Add(payment);

            return payment.ToPostResponse();
        }
    }
}
