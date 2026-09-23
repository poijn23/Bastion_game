using System;

namespace Bastion.Presentation.Utils;

// One account that "already exists", so the login and registration flows can
// be walked end to end before Service and DataAccess are written. Nothing here
// survives that: the checks move behind the REGISTER and LOGIN requests and
// this file is deleted. The ".test" domain is reserved and never resolves.
public static class TestAccount
{
    public const string Email = "prueba@bastion.test";
    public const string Password = "prueba123";
    public const string FriendCode = "AV-4K92";

    // Two more stand in accounts, so the branches CU-01 describes can be
    // walked without a server deciding them.
    public const string PendingIdentifier = "pendiente";
    public const string BannedIdentifier = "sancionado";

    public static string Nickname { get; set; } = "prueba";

    public static bool IsNicknameTaken(string nickname)
    {
        return string.Equals(nickname, Nickname, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsEmailTaken(string email)
    {
        return string.Equals(email, Email, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPending(string identifier)
    {
        return string.Equals(identifier, PendingIdentifier, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsBanned(string identifier)
    {
        return string.Equals(identifier, BannedIdentifier, StringComparison.OrdinalIgnoreCase);
    }

    // The identifier is the nickname or the email, as CU-01 allows both.
    public static bool Matches(string identifier, string password)
    {
        bool isKnown = IsNicknameTaken(identifier) || IsEmailTaken(identifier);

        return isKnown && string.Equals(password, Password, StringComparison.Ordinal);
    }
}
