using System.Runtime.Serialization;

namespace Bastion.Managers;

// Por que se acepto o se rechazo un alta, en los terminos sobre los que el
// formulario puede actuar.
[DataContract]
public enum RegistrationOutcome
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
    Rejected,

    // Solo lo produce el cliente: el servidor no contesto. Vive aqui porque
    // el formulario lo trata como un desenlace mas del intento.
    [EnumMember]
    Unreachable
}
