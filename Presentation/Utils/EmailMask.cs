using System;

namespace Bastion.Presentation.Utils;

public static class EmailMask
{
    private const int MaxHidden = 6;

    public static string Apply(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        int at = email.IndexOf('@');

        if (at < 2)
        {
            return email;
        }

        string local = email[..at];
        int hidden = Math.Min(local.Length - 2, MaxHidden);

        return local[0] + new string('*', hidden) + local[^1] + email[at..];
    }
}
