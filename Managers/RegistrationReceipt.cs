using System.Runtime.Serialization;

namespace Bastion.Managers;

[DataContract]
public sealed record RegistrationReceipt
{
    [DataMember]
    public required RegistrationOutcome Outcome { get; init; }

    [DataMember]
    public string FriendCode { get; init; } = string.Empty;

    public static RegistrationReceipt Refused(RegistrationOutcome outcome)
    {
        return new RegistrationReceipt { Outcome = outcome };
    }
}
