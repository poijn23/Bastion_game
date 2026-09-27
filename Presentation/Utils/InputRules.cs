using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bastion.Presentation.Utils;

// Shape checks the screens run before sending anything, so the obvious
// mistakes come back at once instead of after a round trip (CU-02 FA-03 and
// FA-04). The server repeats them: nothing here is trusted on its own.
public static partial class InputRules
{
    private const int MinNicknameLength = 3;
    private const int MaxNicknameLength = 30;
    private const int MinPasswordLength = 8;
    private const int EarliestBirthYear = 1900;

    private static readonly string[] _shorteners =
        ["bit.ly", "tinyurl.com", "t.co", "goo.gl", "cutt.ly", "rb.gy", "is.gd", "ow.ly"];

    // The shapes are regular expressions and not hand written loops because
    // the pattern is the rule: reading it says what is allowed.
    [GeneratedRegex(@"^[A-Za-z0-9_]{3,30}$")]
    private static partial Regex NicknamePattern();

    // Deliberately stricter than the grammar of an address: one at sign, a
    // host with a dot, and no spaces. What no mail server would deliver to is
    // rejected here instead of after the round trip.
    [GeneratedRegex(@"^[^@\s]+@[^@\s.]+(?:\.[^@\s.]+)+$")]
    private static partial Regex EmailPattern();

    // Unicode letters, because the people who play are called Nuñez and
    // O'Brien. Marks are allowed for the accents that compose.
    [GeneratedRegex(@"^\p{L}[\p{L}\p{M}'\- ]{0,49}$")]
    private static partial Regex PersonNamePattern();

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex DigitsPattern();

    [GeneratedRegex("[a-z]")]
    private static partial Regex LowerPattern();

    [GeneratedRegex("[A-Z]")]
    private static partial Regex UpperPattern();

    [GeneratedRegex(@"\d")]
    private static partial Regex DigitPattern();

    [GeneratedRegex(@"[^A-Za-z0-9]")]
    private static partial Regex SymbolPattern();

    public static bool IsNickname(string nickname)
    {
        ArgumentNullException.ThrowIfNull(nickname);

        return NicknamePattern().IsMatch(nickname);
    }

    public static bool IsPersonName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return PersonNamePattern().IsMatch(name.Trim());
    }

    public static bool IsWholeNumber(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return DigitsPattern().IsMatch(text);
    }

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

    public static bool IsEmail(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return EmailPattern().IsMatch(email);
    }

    public static bool TryParseBirthDate(BirthDateFields fields, out DateOnly date)
    {
        date = default;

        // Digits only: int.TryParse would take " 12" and "+12", which the
        // three boxes of a date never mean.
        if (!IsWholeNumber(fields.Day) || !IsWholeNumber(fields.Month) || !IsWholeNumber(fields.Year))
        {
            return false;
        }

        if (!int.TryParse(fields.Day, out int d)
            || !int.TryParse(fields.Month, out int m)
            || !int.TryParse(fields.Year, out int y))
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
        groups += LowerPattern().IsMatch(password) ? 1 : 0;
        groups += UpperPattern().IsMatch(password) ? 1 : 0;
        groups += DigitPattern().IsMatch(password) ? 1 : 0;
        groups += SymbolPattern().IsMatch(password) ? 1 : 0;

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
            && _shorteners.Contains(address.Host, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsInFuture(DateOnly date)
    {
        return date > DateOnly.FromDateTime(DateTime.Today);
    }

    // Whole years only: a birth date is never off by a fraction of a year for
    // the minimum-age check (CON-12, CU-02 RN-05, CU-07 RN-05).
    public static bool IsAtLeastYearsOld(DateOnly birthDate, int years)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly threshold = birthDate.AddYears(years);

        return threshold <= today;
    }
}
