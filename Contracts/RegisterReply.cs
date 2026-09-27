using System.Runtime.Serialization;

namespace Bastion.Contracts;

[DataContract]
public sealed record RegisterReply
{
    [DataMember]
    public required RegisterOutcome Outcome { get; init; }

    [DataMember]
    public string FriendCode { get; init; } = string.Empty;
}
