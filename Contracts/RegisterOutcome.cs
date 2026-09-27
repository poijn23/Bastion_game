using System.Runtime.Serialization;

namespace Bastion.Contracts;

// Why the server said no, in the words the form can act on. Anything the
// person cannot fix arrives as Rejected.
[DataContract]
public enum RegisterOutcome
{
    [EnumMember]
    Registered,

    [EnumMember]
    NicknameTaken,

    [EnumMember]
    EmailTaken,

    [EnumMember]
    UnderageRejected,

    [EnumMember]
    TermsNotAccepted,

    [EnumMember]
    Rejected
}
