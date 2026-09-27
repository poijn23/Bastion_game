namespace Bastion.Presentation.Utils;

// What the form does about the answer. It mirrors the outcomes the contract
// declares and adds the one the server cannot report: not reaching it.
public enum RegistrationAnswer
{
    Registered,
    NicknameTaken,
    EmailTaken,
    Underage,
    TermsNotAccepted,
    Rejected,
    Unreachable
}
