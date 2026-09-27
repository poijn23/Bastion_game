namespace Bastion.DataAccess;

// What the service asks of the database about accounts. The service depends
// on this and never on SqlAccountRepository (rule 10.1).
public interface IAccountRepository
{
    Task<RegistrationResult> RegisterAsync(NewAccount account, CancellationToken cancellation);

    Task<bool> IsNicknameTakenAsync(string nickname, CancellationToken cancellation);

    Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellation);
}
