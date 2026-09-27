using Bastion.Domain;

namespace Bastion.Managers;

// CU-02. Decide lo que no necesita base (edad y terminos), deriva la
// contrasena, sortea el codigo de amigo y le pide al DAO que escriba. El
// cliente propone y esto decide (DRV-01).
public sealed class AccountManager : IAccountManager
{
    private const int MinimumAge = 8;
    private const int FriendCodeAttempts = 5;
    private const string TermsVersion = "2026.1";

    // La direccion desde la que se conecto pertenece a la fila de auditoria.
    // Hasta que el host la entregue, la fila lo dice sin disimulo.
    private const string UnknownAddress = "0.0.0.0";

    private readonly IAccountDao _accounts;

    public AccountManager(IAccountDao accounts)
    {
        ArgumentNullException.ThrowIfNull(accounts);

        _accounts = accounts;
    }

    public async Task<RegistrationReceipt> RegisterAsync(
        AccountRegistration registration, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(registration);

        RegistrationOutcome? refusal = Refuse(registration);

        if (refusal is not null)
        {
            return RegistrationReceipt.Refused(refusal.Value);
        }

        StoredPassword password = PasswordHasher.Hash(registration.Password);

        // Un choque de codigo de amigo es mala suerte del servidor, no algo
        // que la persona pueda arreglar, asi que vuelve a sortear.
        for (int attempt = 0; attempt < FriendCodeAttempts; attempt++)
        {
            RegistrationReceipt? receipt = await TryWriteAsync(registration, password, cancellation);

            if (receipt is not null)
            {
                return receipt;
            }
        }

        return RegistrationReceipt.Refused(RegistrationOutcome.Rejected);
    }

    public async Task<bool> IsNicknameTakenAsync(string nickname, CancellationToken cancellation)
    {
        return await _accounts.IsNicknameTakenAsync(nickname, cancellation);
    }

    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellation)
    {
        return await _accounts.IsEmailTakenAsync(email, cancellation);
    }

    private async Task<RegistrationReceipt?> TryWriteAsync(
        AccountRegistration registration, StoredPassword password, CancellationToken cancellation)
    {
        string friendCode = FriendCode.Next();
        AccountInsertResult written = await _accounts.InsertAsync(
            Describe(registration, password, friendCode), cancellation);

        return written.Result switch
        {
            AccountInsert.Written => new RegistrationReceipt
            {
                Outcome = RegistrationOutcome.Registered,
                FriendCode = friendCode
            },
            AccountInsert.NicknameTaken => RegistrationReceipt.Refused(RegistrationOutcome.NicknameTaken),
            AccountInsert.EmailTaken => RegistrationReceipt.Refused(RegistrationOutcome.EmailTaken),
            _ => null
        };
    }

    private static AccountRow Describe(
        AccountRegistration registration, StoredPassword password, string friendCode)
    {
        return new AccountRow
        {
            FirstName = registration.FirstName.Trim(),
            Surnames = registration.Surnames.Trim(),
            Nickname = registration.Nickname.Trim(),
            Email = registration.Email.Trim(),
            PasswordHash = password.Hash,
            PasswordSalt = password.Salt,
            BirthDate = registration.BirthDate,
            Language = registration.Language,
            FriendCode = friendCode,
            TermsVersion = TermsVersion,
            Address = UnknownAddress
        };
    }

    // Lo que el servidor rechaza por su cuenta, antes de tocar la base. La
    // edad esta aqui y no en un CHECK porque D-06 la cuenta contra hoy.
    private static RegistrationOutcome? Refuse(AccountRegistration registration)
    {
        if (!registration.AcceptsTerms)
        {
            return RegistrationOutcome.TermsNotAccepted;
        }

        return IsOldEnough(registration.BirthDate) ? null : RegistrationOutcome.UnderageRejected;
    }

    private static bool IsOldEnough(DateOnly birthDate)
    {
        return birthDate <= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-MinimumAge);
    }
}
