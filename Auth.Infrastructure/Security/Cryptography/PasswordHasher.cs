using Auth.Domain.Users.Interfaces;

namespace Auth.Infrastructure.Security.Cryptography;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("A senha não pode estar vázia ou conter apenas espaços.", nameof(password));
        
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrEmpty(hashedPassword))
            return false;

        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}