using System;
using Bastion.Presentation.GUI_AccountSettings;
using Bastion.Presentation.GUI_ActiveSessions;
using Bastion.Presentation.GUI_ChangeEmail;
using Bastion.Presentation.GUI_ChangeNickname;
using Bastion.Presentation.GUI_ChangePassword;
using Bastion.Presentation.GUI_DeleteAccount;
using Bastion.Presentation.GUI_EditProfile;
using Bastion.Presentation.GUI_ForgotPassword;
using Bastion.Presentation.GUI_Login;
using Bastion.Presentation.GUI_MessageConfirm;
using Bastion.Presentation.GUI_MessageError;
using Bastion.Presentation.GUI_MessageSuccess;
using Bastion.Presentation.GUI_MessageWarning;
using Bastion.Presentation.GUI_Profile;
using Bastion.Presentation.GUI_Register;
using Bastion.Presentation.GUI_RegistrationSuccess;
using Bastion.Presentation.GUI_ResetPassword;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation;

// Lives outside Utils on purpose: it is the only place that knows every
// concrete screen, so Utils never depends on the GUI packages.
public sealed class Navigator : INavigator
{
    private IScreen? _current;
    private IScreen? _dialog;
    private ScreenId _currentId;
    private ScreenId _previousId;
    private string? _currentArgument;
    private string? _previousArgument;
    private Action? _pendingConfirm;

    public void Start(ScreenId screen)
    {
        _currentId = screen;
        _previousId = screen;
        _current = Build(screen, null);
        _dialog = null;
    }

    public void GoTo(ScreenId screen, string? argument = null)
    {
        _previousId = _currentId;
        _previousArgument = _currentArgument;
        _currentId = screen;
        _currentArgument = argument;
        _current = Build(screen, argument);
        _dialog = null;
    }

    public void GoBack()
    {
        GoTo(_previousId, _previousArgument);
    }

    public void ShowConfirm(ConfirmRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var confirm = new GuiMessageConfirm();
        confirm.ShowWithLabels(request.Body, GetPrimaryLabel(request), GetSecondaryLabel(request));
        confirm.Dialog.PrimaryChosen += OnConfirmAccepted;
        confirm.Dialog.SecondaryChosen += OnDialogDismissed;

        _pendingConfirm = request.OnConfirm;
        _dialog = confirm;
    }

    public void ShowMessage(DialogTone tone, string body)
    {
        ArgumentNullException.ThrowIfNull(body);

        MessageScreen screen = BuildMessage(tone);
        screen.Show(body);
        screen.Dialog.PrimaryChosen += OnDialogDismissed;
        _dialog = screen;
    }

    // The dialog draws its own backdrop over the screen underneath, so both are
    // drawn but only the topmost one receives input.
    public void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (_dialog is not null)
        {
            _dialog.Update(input);
            return;
        }

        _current?.Update(input);
    }

    public void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        _current?.Draw(canvas);
        _dialog?.Draw(canvas);
    }

    private void CloseDialog()
    {
        _dialog = null;
        _pendingConfirm = null;
    }

    private void OnDialogDismissed(object? sender, EventArgs e)
    {
        CloseDialog();
    }

    private void OnConfirmAccepted(object? sender, EventArgs e)
    {
        Action? confirmed = _pendingConfirm;
        CloseDialog();
        confirmed?.Invoke();
    }

    private IScreen Build(ScreenId screen, string? argument)
    {
        return screen switch
        {
            ScreenId.Register => new GuiRegister(this),
            ScreenId.RegistrationSuccess => new GuiRegistrationSuccess(this, argument ?? string.Empty),
            ScreenId.Profile => new GuiProfile(this),
            ScreenId.EditProfile => new GuiEditProfile(this),
            ScreenId.ForgotPassword => new GuiForgotPassword(this),
            ScreenId.ResetPassword => new GuiResetPassword(this),
            ScreenId.AccountSettings => new GuiAccountSettings(this),
            ScreenId.ChangePassword => new GuiChangePassword(this),
            ScreenId.ChangeEmail => new GuiChangeEmail(this),
            ScreenId.ChangeNickname => new GuiChangeNickname(this),
            ScreenId.DeleteAccount => new GuiDeleteAccount(this),
            ScreenId.ActiveSessions => new GuiActiveSessions(this),
            ScreenId.Login => new GuiLogin(this),
            _ => new GuiLogin(this)
        };
    }

    private static MessageScreen BuildMessage(DialogTone tone)
    {
        return tone switch
        {
            DialogTone.Error => new GuiMessageError(),
            DialogTone.Success => new GuiMessageSuccess(),
            DialogTone.Confirm => new GuiMessageWarning(),
            DialogTone.Warning => new GuiMessageWarning(),
            _ => new GuiMessageWarning()
        };
    }

    private static string GetPrimaryLabel(ConfirmRequest request)
    {
        return string.IsNullOrEmpty(request.PrimaryLabel)
            ? TextCatalog.DialogConfirmButton
            : request.PrimaryLabel;
    }

    private static string GetSecondaryLabel(ConfirmRequest request)
    {
        return string.IsNullOrEmpty(request.SecondaryLabel)
            ? TextCatalog.DialogCancelButton
            : request.SecondaryLabel;
    }
}
