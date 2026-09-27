namespace Bastion.Presentation.Utils;

// Answers from the stand in account, so the client runs, and the tests run,
// with no server listening. It is what TestAccount was already doing, now
// behind the same door the real server uses.
public sealed class OfflineAccountGateway : IAccountGateway
{
    public RegistrationAnswer Register(NewAccountRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!request.AcceptsTerms)
        {
            return RegistrationAnswer.TermsNotAccepted;
        }

        if (IsNicknameTaken(request.Nickname))
        {
            return RegistrationAnswer.NicknameTaken;
        }

        return IsEmailTaken(request.Email) ? RegistrationAnswer.EmailTaken : RegistrationAnswer.Registered;
    }

    public bool IsNicknameTaken(string nickname)
    {
        return TestAccount.IsNicknameTaken(nickname);
    }

    public bool IsEmailTaken(string email)
    {
        return TestAccount.IsEmailTaken(email);
    }
}
