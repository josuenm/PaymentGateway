using Checkout.Domain.Checkouts.Entities;

namespace Checkout.Domain.Checkouts.Repositories;

public interface ICheckoutSessionRepository
{
    public Task<CheckoutSession> CreateAsync(CheckoutSession session);
    public Task<CheckoutSession?> GetById(string sessionId);
}