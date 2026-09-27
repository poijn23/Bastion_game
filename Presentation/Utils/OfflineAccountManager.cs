using System;
using System.Threading;
using System.Threading.Tasks;
using Bastion.Managers;

namespace Bastion.Presentation.Utils;

// Contesta desde la cuenta sustituta, para que el cliente y las pruebas
// funcionen sin nada escuchando. Es lo que TestAccount ya hacia, ahora
// detras de la misma interfaz que usa el servidor.
public sealed class OfflineAccountManager : IAccountManager
{
    public Task<RegistrationReceipt> RegisterAsync(
        AccountRegistration registration, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(registration);

        return Task.FromResult(Decide(registration));
    }

    public Task<bool> IsNicknameTakenAsync(string nickname, CancellationToken cancellation)
    {
        return Task.FromResult(TestAccount.IsNicknameTaken(nickname));
    }

    public Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellation)
    {
        return Task.FromResult(TestAccount.IsEmailTaken(email));
    }

    private static RegistrationReceipt Decide(AccountRegistration registration)
    {
        if (!registration.AcceptsTerms)
        {
            return RegistrationReceipt.Refused(RegistrationOutcome.TermsNotAccepted);
        }

        if (TestAccount.IsNicknameTaken(registration.Nickname))
        {
            return RegistrationReceipt.Refused(RegistrationOutcome.NicknameTaken);
        }

        return TestAccount.IsEmailTaken(registration.Email)
            ? RegistrationReceipt.Refused(RegistrationOutcome.EmailTaken)
            : new RegistrationReceipt { Outcome = RegistrationOutcome.Registered };
    }
}
