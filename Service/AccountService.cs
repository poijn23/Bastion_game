using CoreWCF;
using Bastion.Contracts;
using Bastion.DataAccess;
using Bastion.Domain;

namespace Bastion.Service;

/// <summary>
/// Serves CU-02: it decides whether an account may be created and asks the
/// repository to write it. The client proposes, this decides (DRV-01).
/// </summary>
[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
public sealed class AccountService : IAccountService
{
    private const int MinimumAge = 8;
    private const string TermsVersion = "2026.1";

    private readonly IAccountRepository _accounts;

    /// <summary>Takes the repository by interface, never a connection.</summary>
    public AccountService(IAccountRepository accounts)
    {
        ArgumentNullException.ThrowIfNull(accounts);

        _accounts = accounts;
    }

    /// <summary>Creates a pending account, its accepted terms and its verification token.</summary>
    public async Task<RegisterReply> RegisterAsync(RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        RegisterOutcome? refusal = Refuse(request);

        if (refusal is not null)
        {
            return new RegisterReply { Outcome = refusal.Value };
        }

        RegistrationResult result = await _accounts.RegisterAsync(Describe(request), CancellationToken.None);

        return new RegisterReply { Outcome = Translate(result.Outcome), FriendCode = result.FriendCode };
    }

    /// <summary>Answers the form while the person is still typing the nickname.</summary>
    public async Task<bool> IsNicknameTakenAsync(string nickname)
    {
        return await _accounts.IsNicknameTakenAsync(nickname, CancellationToken.None);
    }

    /// <summary>Answers the form while the person is still typing the address.</summary>
    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await _accounts.IsEmailTakenAsync(email, CancellationToken.None);
    }

    // What the server refuses on its own, before touching the database. The
    // age is here and not in a CHECK because D-06 counts it against today.
    private static RegisterOutcome? Refuse(RegisterRequest request)
    {
        if (!request.AcceptsTerms)
        {
            return RegisterOutcome.TermsNotAccepted;
        }

        return IsOldEnough(request.BirthDate) ? null : RegisterOutcome.UnderageRejected;
    }

    private static bool IsOldEnough(DateOnly birthDate)
    {
        return birthDate <= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-MinimumAge);
    }

    private static NewAccount Describe(RegisterRequest request)
    {
        return new NewAccount
        {
            Nickname = request.Nickname.Trim(),
            Email = request.Email.Trim(),
            Password = PasswordHasher.Hash(request.Password),
            BirthDate = request.BirthDate,
            Language = request.Language,

            // The address the person connected from belongs in the audit row.
            // Until the host hands it over, the record says so plainly.
            TermsVersion = TermsVersion,
            Address = "0.0.0.0"
        };
    }

    private static RegisterOutcome Translate(RegistrationOutcome outcome)
    {
        return outcome switch
        {
            RegistrationOutcome.Registered => RegisterOutcome.Registered,
            RegistrationOutcome.NicknameTaken => RegisterOutcome.NicknameTaken,
            RegistrationOutcome.EmailTaken => RegisterOutcome.EmailTaken,
            _ => RegisterOutcome.Rejected
        };
    }
}
