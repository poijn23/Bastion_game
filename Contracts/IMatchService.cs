using System.ServiceModel;

namespace Bastion.Contracts;

[ServiceContract(CallbackContract = typeof(IMatchCallback), SessionMode
= SessionMode.Required)]

public interface ImatchService
{
    [OperationContract]
    Task<LoginResult> LogInAsync(string identifier, string password);

    [OperationContract(IsOneWay = true)]
    Task MovePawnAsync(Guid matchId, int row, int column);
}