namespace Bastion.Managers;

// Lo que se puede hacer con una cuenta. El controlador depende de esto y
// nunca de quien lo implemente: en el servidor la logica real, en el cliente
// el que la alcanza por la red.
public interface IAccountManager
{
    Task<RegistrationReceipt> RegisterAsync(AccountRegistration registration, CancellationToken cancellation);

    Task<bool> IsNicknameTakenAsync(string nickname, CancellationToken cancellation);

    Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellation);
}
