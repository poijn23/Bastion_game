using System;
using System.Net.Mail;

namespace Bastion.Presentation.Utils;

// Shape checks the screens run before sending anything, so the obvious
// mistakes come back at once instead of after a round trip (CU-02 FA-03 and
// FA-04). The server repeats them: nothing here is trusted on its own.
public static class InputRules
{
    private const int MinNicknameLength = 3;
    private const int MaxNicknameLength = 30;
    private const int MinPasswordLength = 8;
    private const int EarliestBirthYear = 1900;

    public static bool HasNicknameLength(string nickname)
    {
        ArgumentNullException.ThrowIfNull(nickname);

        return nickname.Length >= MinNicknameLength && nickname.Length <= MaxNicknameLength;
    }

    public static bool HasPasswordLength(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        return password.Length >= MinPasswordLength;
    }

    // MailAddress accepts "a@b"; requiring a dot in the host keeps out the
    // addresses no mail server would deliver to anyway.
    public static bool IsEmail(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return MailAddress.TryCreate(email, out MailAddress? address)
            && address.Host.Contains('.', StringComparison.Ordinal)
            && !address.Host.EndsWith('.');
    }

    public static bool TryParseBirthDate(string day, string month, string year, out DateOnly date)
    {
        date = default;

        if (!int.TryParse(day, out int d) || !int.TryParse(month, out int m) || !int.TryParse(year, out int y))
        {
            return false;
        }

        if (y < EarliestBirthYear || m < 1 || m > 12 || d < 1 || d > DateTime.DaysInMonth(y, m))
        {
            return false;
        }

        date = new DateOnly(y, m, d);
        return true;
    }

    public static bool IsInFuture(DateOnly date)
    {
        return date > DateOnly.FromDateTime(DateTime.Today);
    }
}
