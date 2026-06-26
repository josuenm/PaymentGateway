using Auth.Domain.RefreshTokens.Entities;

namespace Auth.Domain.RefreshTokens.Repositories;

public interface IRefreshTokenRepository
{
    public Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
}