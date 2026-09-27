using CoreWCF;
using Bastion.Contracts;
using Bastion.Managers;

namespace Bastion.Service;

/// <summary>
/// Punto de entrada remoto del CU-02. No decide nada: recibe el mensaje y se
/// lo pasa al manager, que es quien tiene la logica.
/// </summary>
[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
public sealed class AccountService : IAccountService
{
    private readonly IAccountManager _accounts;

    /// <summary>Recibe el manager por interfaz, nunca un DAO ni una conexion.</summary>
    public AccountService(IAccountManager accounts)
    {
        ArgumentNullException.ThrowIfNull(accounts);

        _accounts = accounts;
    }

    /// <summary>Da de alta una cuenta pendiente de verificar.</summary>
    public async Task<RegistrationReceipt> RegisterAsync(AccountRegistration registration)
    {
        return await _accounts.RegisterAsync(registration, CancellationToken.None);
    }

    /// <summary>Contesta al formulario mientras se escribe el nickname.</summary>
    public async Task<bool> IsNicknameTakenAsync(string nickname)
    {
        return await _accounts.IsNicknameTakenAsync(nickname, CancellationToken.None);
    }

    /// <summary>Contesta al formulario mientras se escribe el correo.</summary>
    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await _accounts.IsEmailTakenAsync(email, CancellationToken.None);
    }
}
