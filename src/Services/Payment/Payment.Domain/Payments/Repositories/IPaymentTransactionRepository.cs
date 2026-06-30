using Payment.Domain.Payments.Entities;

namespace Payment.Domain.Payments.Repositories;

public interface IPaymentTransactionRepository
{
    public Task<PaymentTransactionEntity> CreateAsync(PaymentTransactionEntity paymentTransaction);
    public Task<bool> SetPaidByIdAsync(PaymentTransactionEntity paymentTransaction);
    public Task<PaymentTransactionEntity?> GetByIdAsync(string id);
}