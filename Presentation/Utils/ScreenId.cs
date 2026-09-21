namespace Bastion.Presentation.Utils;

// The screens the navigator can reach by name, so a screen asks for a
// destination without depending on the class that implements it.
public enum ScreenId
{
    Login,
    Register,
    ForgotPassword,
    ResetPassword,
    AccountSettings,
    ChangePassword,
    ChangeEmail,
    ChangeNickname,
    DeleteAccount,
    ActiveSessions,
    RegistrationSuccess,
    Profile,
    EditProfile
}
