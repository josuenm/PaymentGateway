using Auth.Domain.RefreshTokens.Entities;
using Auth.Domain.RefreshTokens.Repositories;
using Shared.Infrastructure.Contexts;
using Dapper;

namespace Auth.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly DapperContext _context;
    
    public RefreshTokenRepository(DapperContext context)
    {
        _context = context;
    }
    
    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        string sql = 
@"
INSERT INTO RefreshTokens (Id, Token, UserId, IsRevoked, ExpiresAt, CreatedAt) 
VALUES (@Id, @Token, @UserId, @IsRevoked, @ExpiresAt, @CreatedAt)
";
        
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(sql, refreshToken);
            
        return refreshToken;
    }
}