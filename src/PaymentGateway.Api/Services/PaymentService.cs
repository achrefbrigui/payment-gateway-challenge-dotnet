using PaymentGateway.Api.Mappers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService( IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PostPaymentResponse> ProcessPayment(PostPaymentRequest request)
        {
            var payment = request.ToDomain();

            //// TODO : implement bank client

            _paymentRepository.Add(payment);

            return payment.ToPostResponse();
        }
    }
}
