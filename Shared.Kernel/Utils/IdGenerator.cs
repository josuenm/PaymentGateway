namespace Shared.Kernel.Utils;

using System.Security.Cryptography;

public static class IdGenerator
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string Generate(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("O prefixo não pode ser vazio.", nameof(prefix));

        var chars = new char[16]; 
        var bytes = new byte[16];

        RandomNumberGenerator.Fill(bytes);

        for (int i = 0; i < bytes.Length; i++)
        {
            chars[i] = Alphabet[bytes[i] % Alphabet.Length];
        }

        return $"{prefix}_{new string(chars)}";
    }
}