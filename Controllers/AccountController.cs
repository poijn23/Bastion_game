using Bastion.Managers;

namespace Bastion.Controllers;

// Punto de entrada de las pantallas de cuenta. No decide nada: lo unico que
// hace de suyo es esperar la respuesta, porque el bucle de la interfaz es
// sincrono y un alta ocurre una vez, no por fotograma.
public sealed class AccountController
{
    private readonly IAccountManager _accounts;

    public AccountController(IAccountManager accounts)
    {
        ArgumentNullException.ThrowIfNull(accounts);

        _accounts = accounts;
    }

    public RegistrationReceipt Register(AccountRegistration registration)
    {
        return Wait(_accounts.RegisterAsync(registration, CancellationToken.None));
    }

    public bool IsNicknameTaken(string nickname)
    {
        return Wait(_accounts.IsNicknameTakenAsync(nickname, CancellationToken.None));
    }

    public bool IsEmailTaken(string email)
    {
        return Wait(_accounts.IsEmailTakenAsync(email, CancellationToken.None));
    }

    private static TAnswer Wait<TAnswer>(Task<TAnswer> call)
    {
        return call.GetAwaiter().GetResult();
    }
}
