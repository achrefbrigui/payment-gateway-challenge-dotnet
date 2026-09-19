using PaymentGateway.Api.Domain;

namespace PaymentGateway.Api.Services
{
    public interface IPaymentRepository
    {
        void Add(Payment payment);
        Payment? Get(Guid id);
    }
}
