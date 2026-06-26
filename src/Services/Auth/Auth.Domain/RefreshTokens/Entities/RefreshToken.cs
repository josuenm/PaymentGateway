using Shared.Kernel.Utils;

namespace Auth.Domain.RefreshTokens.Entities;

public class RefreshToken
{
    public string Id { get; private set; }
    public string Token { get; private set; }
    public string UserId { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public RefreshToken(string id, string token, string userId, bool isRevoked, DateTime expiresAt, DateTime createdAt)
    {
        Id = id;
        Token = token;
        UserId = userId;
        IsRevoked = isRevoked;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
    }
    
    public RefreshToken(string token, string userId, bool isRevoked, DateTime expiresAt)
    {
        Id = IdGenerator.Generate("reft");
        Token = token;
        UserId = userId;
        IsRevoked = isRevoked;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        IsRevoked = true;
    }
    
    private RefreshToken() {}
}