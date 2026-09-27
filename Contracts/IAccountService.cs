using System.ServiceModel;
using Bastion.Managers;

namespace Bastion.Contracts;

// La cara remota de IAccountManager: las mismas operaciones, con lo que el
// transporte necesita saber. Los DTO son los de la capa de logica.
[ServiceContract]
public interface IAccountService
{
    [OperationContract]
    Task<RegistrationReceipt> RegisterAsync(AccountRegistration registration);

    [OperationContract]
    Task<bool> IsNicknameTakenAsync(string nickname);

    [OperationContract]
    Task<bool> IsEmailTakenAsync(string email);
}
