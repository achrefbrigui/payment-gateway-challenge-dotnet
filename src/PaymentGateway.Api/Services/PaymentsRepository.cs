using PaymentGateway.Api.Domain;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository : IPaymentRepository
{
    public Dictionary<Guid, Payment> Payments = new();
    
    public void Add(Payment payment)
    {
        Payments[payment.Id] = payment;
    }

    public Payment? Get(Guid id)
    {
        return Payments.GetValueOrDefault(id);
    }
}