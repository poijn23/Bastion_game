using System.ServiceModel;

namespace Bastion.Contracts;

// CU-02. The only thing the registration form is allowed to ask the server.
[ServiceContract]
public interface IAccountService
{
    [OperationContract]
    Task<RegisterReply> RegisterAsync(RegisterRequest request);

    [OperationContract]
    Task<bool> IsNicknameTakenAsync(string nickname);

    [OperationContract]
    Task<bool> IsEmailTakenAsync(string email);
}
