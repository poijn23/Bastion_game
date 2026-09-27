namespace Bastion.Presentation.Utils;

// The way out of the client for anything about accounts. The screen depends
// on this and never on a channel or a connection string (rule 10.1).
public interface IAccountGateway
{
    RegistrationAnswer Register(NewAccountRequest request);

    bool IsNicknameTaken(string nickname);

    bool IsEmailTaken(string email);
}
