using Checkout.Domain.PaymentLinkReadModels.Entities;
using Checkout.Domain.PaymentLinkReadModels.Repositories;
using Dapper;
using Shared.Infrastructure.Contexts;

namespace Checkout.Infrastructure.Repositories;

public class PaymentLinkReadModelRepository : IPaymentLinkReadModelRepository
{
    private readonly DapperContext _context;
    private readonly IPaymentLinkItemReadModelRepository _paymentLinkItemReadModelRepository;

    public PaymentLinkReadModelRepository(
        DapperContext dapperContext, 
        IPaymentLinkItemReadModelRepository paymentLinkItemReadModelRepository
    )
    {
        _context = dapperContext;
        _paymentLinkItemReadModelRepository = paymentLinkItemReadModelRepository;
    }

    public async Task CreateAsync(PaymentLinkReadModel readModel)
    {
        const string sql = 
@"
INSERT INTO PaymentLinkReadModel (Id, IsActive, UserId, LiveMode) VALUES (@Id, @IsActive, @UserId, @LiveMode)
";

        using var connection = _context.CreateConnection();
        var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(sql, readModel);

        if (readModel.Items.Any())
        {
            await _paymentLinkItemReadModelRepository.CreateManyAsync(readModel.Items, transaction);
        }

        transaction.Commit();
    }

    public async Task<PaymentLinkReadModel?> GetByIdAsync(string id, bool includeItems = false)
    {
        const string sql = "SELECT * FROM PaymentLinkReadModel WHERE Id = @Id";
        object parameters = new { Id = id };
        
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<PaymentLinkReadModel>(sql, parameters);
    }
}