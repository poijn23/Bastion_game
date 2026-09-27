using System.Security.Cryptography;

namespace Bastion.Domain;

// The code a player shares to be added. UX_Usuario_codigo_amigo makes it
// unique, so whoever stores it retries when the database rejects a repeat.
public static class FriendCode
{
    public const int Length = 8;

    // No I, O, 0 or 1: the code is read out loud and copied by hand.
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static string Next()
    {
        char[] code = new char[Length];

        for (int index = 0; index < code.Length; index++)
        {
            code[index] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        return new string(code);
    }
}
