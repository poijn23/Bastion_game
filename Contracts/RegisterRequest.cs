using System.Runtime.Serialization;

namespace Bastion.Contracts;

// What the registration form sends. One message instead of seven arguments,
// which also keeps the operation inside the three parameter limit.
[DataContract]
public sealed record RegisterRequest
{
    [DataMember]
    public required string Nickname { get; init; }

    [DataMember]
    public required string Email { get; init; }

    [DataMember]
    public required string Password { get; init; }

    [DataMember]
    public required DateOnly BirthDate { get; init; }

    [DataMember]
    public required string Language { get; init; }

    [DataMember]
    public required bool AcceptsTerms { get; init; }
}
