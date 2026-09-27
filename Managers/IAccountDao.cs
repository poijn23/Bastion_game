namespace Bastion.Managers;

// El puerto que la capa de logica necesita de la base. Lo implementa
// DataAccess y no tiene mas que peticiones: ninguna decision vive aqui.
public interface IAccountDao
{
    Task<AccountInsertResult> InsertAsync(AccountRow row, CancellationToken cancellation);

    Task<bool> IsNicknameTakenAsync(string nickname, CancellationToken cancellation);

    Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellation);
}
