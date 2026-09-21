using System;
using System.Linq;
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

    private static readonly string[] Shorteners =
        ["bit.ly", "tinyurl.com", "t.co", "goo.gl", "cutt.ly", "rb.gy", "is.gd", "ow.ly"];

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

    public static int CountPasswordGroups(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        int groups = 0;
        groups += password.Any(char.IsLower) ? 1 : 0;
        groups += password.Any(char.IsUpper) ? 1 : 0;
        groups += password.Any(char.IsDigit) ? 1 : 0;
        groups += password.Any(c => !char.IsLetterOrDigit(c)) ? 1 : 0;

        return groups;
    }

    public static bool MeetsPasswordPolicy(string password)
    {
        return HasPasswordLength(password) && CountPasswordGroups(password) >= 3;
    }

    public static int PasswordStrength(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (password.Length == 0)
        {
            return 0;
        }

        if (!HasPasswordLength(password))
        {
            return 1;
        }

        return CountPasswordGroups(password) >= 3 ? 3 : 2;
    }

    public static bool IsWebAddress(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        return Uri.TryCreate(url, UriKind.Absolute, out Uri? address)
            && address.Scheme == Uri.UriSchemeHttps
            && address.Host.Contains('.', StringComparison.Ordinal);
    }

    public static bool IsShortener(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        return Uri.TryCreate(url, UriKind.Absolute, out Uri? address)
            && Shorteners.Contains(address.Host, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsInFuture(DateOnly date)
    {
        return date > DateOnly.FromDateTime(DateTime.Today);
    }
}
