using System.Text.Json;
using Dapper;
using PaymentLink.Domain.Entities;
using PaymentLink.Domain.PaymentLinks.Repositories;
using Shared.Infrastructure.Contexts;

namespace PaymentLink.Infrastructure.Repositories;

public class PaymentLinkRepository : IPaymentLinkRepository
{
    private readonly DapperContext _context;
    private readonly IPaymentLinkItemRepository _paymentLinkItemRepository;
    
    public PaymentLinkRepository(DapperContext context, IPaymentLinkItemRepository paymentLinkItemRepository)
    {
        _context = context;
        _paymentLinkItemRepository = paymentLinkItemRepository;
    }

    public async Task<PaymentLinkEntity> CreateAsync(PaymentLinkEntity paymentLink)
    {
        const string sql =
@"
INSERT INTO PaymentLinks (Id, Methods, UserId, LiveMode, IsActive, CreatedAt)
VALUES (@Id, @Methods, @UserId, @LiveMode, @IsActive, @CreatedAt)
";

        using var connection = _context.CreateConnection();
        var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(
            sql, 
            new
            {
                Id = paymentLink.Id, 
                Methods = JsonSerializer.Serialize(
                    paymentLink.Methods.Select(m => m.ToString()).ToList()
                ), 
                UserId = paymentLink.UserId, 
                LiveMode = paymentLink.LiveMode, 
                IsActive = paymentLink.IsActive, 
                CreatedAt = paymentLink.CreatedAt 
            }, 
            transaction
        );
        if (paymentLink.Items.Any())
        {
            await _paymentLinkItemRepository.CreateManyAsync(paymentLink.Items, transaction, connection);
        }

        transaction.Commit();
        
        return paymentLink;
    }
}