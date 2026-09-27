using System.Runtime.Serialization;

namespace Bastion.Managers;

// Lo que el formulario de registro reunio, tal cual lo escribio la persona.
// Viaja entero porque un alta es una sola operacion (CU-02).
[DataContract]
public sealed record AccountRegistration
{
    [DataMember]
    public required string FirstName { get; init; }

    [DataMember]
    public required string Surnames { get; init; }

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
