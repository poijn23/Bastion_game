using System.Security.Cryptography;
using System.Text;

namespace Bastion.Domain;

// PBKDF2 over SHA-512. The sizes are not a preference: contrasena_hash is
// varbinary(64) and contrasena_sal is varbinary(32), so the derivation has to
// produce exactly that.
public static class PasswordHasher
{
    public const int HashBytes = 64;
    public const int SaltBytes = 32;

    private const int Iterations = 210_000;

    public static StoredPassword Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        byte[] salt = RandomNumberGenerator.GetBytes(SaltBytes);

        return new StoredPassword { Hash = Derive(password, salt), Salt = salt };
    }

    public static bool Matches(string password, StoredPassword stored)
    {
        ArgumentNullException.ThrowIfNull(stored);

        if (string.IsNullOrEmpty(password))
        {
            return false;
        }

        // Fixed time comparison: a length or prefix shortcut would leak how
        // much of the hash was right.
        return CryptographicOperations.FixedTimeEquals(Derive(password, stored.Salt), stored.Hash);
    }

    private static byte[] Derive(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA512,
            HashBytes);
    }
}
