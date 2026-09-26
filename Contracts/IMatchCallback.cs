using System.ServiceModel;

namespace Bastion.Contracts;

public interface IMatchCallback
{
    [OperationContract(IsOneWay = true)]
    Task NotifyMoveMadeAsync(Guid matchId, MoveMade move);

    [OperationContract(IsOneWay = true)]
    Task NotifyOpponentDisconnectAsync(Guid matchId);

}