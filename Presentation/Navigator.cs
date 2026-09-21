using System;
using System.Collections.Generic;
using Bastion.Presentation.GUI_ActiveSessions;
using Bastion.Presentation.GUI_AddFriend;
using Bastion.Presentation.GUI_AdminPanel;
using Bastion.Presentation.GUI_Appeal;
using Bastion.Presentation.GUI_ApplySanction;
using Bastion.Presentation.GUI_BoxPurchaseConfirm;
using Bastion.Presentation.GUI_ChangeEmail;
using Bastion.Presentation.GUI_ChangeNickname;
using Bastion.Presentation.GUI_ChangePassword;
using Bastion.Presentation.GUI_CoinHistory;
using Bastion.Presentation.GUI_Customize;
using Bastion.Presentation.GUI_DeleteAccount;
using Bastion.Presentation.GUI_EditProfile;
using Bastion.Presentation.GUI_ForgotPassword;
using Bastion.Presentation.GUI_Friends;
using Bastion.Presentation.GUI_Login;
using Bastion.Presentation.GUI_Logs;
using Bastion.Presentation.GUI_MatchHistory;
using Bastion.Presentation.GUI_MessageConfirm;
using Bastion.Presentation.GUI_MessageError;
using Bastion.Presentation.GUI_MessageSuccess;
using Bastion.Presentation.GUI_Menu;
using Bastion.Presentation.GUI_MessageWarning;
using Bastion.Presentation.GUI_ModerationQueue;
using Bastion.Presentation.GUI_PlayerCard;
using Bastion.Presentation.GUI_Profile;
using Bastion.Presentation.GUI_PurchaseConfirm;
using Bastion.Presentation.GUI_Ranking;
using Bastion.Presentation.GUI_Register;
using Bastion.Presentation.GUI_RegistrationSuccess;
using Bastion.Presentation.GUI_Report;
using Bastion.Presentation.GUI_ReportReview;
using Bastion.Presentation.GUI_ResetPassword;
using Bastion.Presentation.GUI_Settings;
using Bastion.Presentation.GUI_Shop;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation;

// Lives outside Utils on purpose: it is the only place that knows every
// concrete screen, so Utils never depends on the GUI packages.
public sealed class Navigator : INavigator
{
    // A lookup instead of a switch: CA1502 counts every case label, and
    // thirty of them put Build far over the limit of ten.
    private static readonly Dictionary<ScreenId, Func<INavigator, IScreen>> _factories = new()
    {
        [ScreenId.Login] =
            navigator => new GuiLogin(navigator),
        [ScreenId.Register] =
            navigator => new GuiRegister(navigator),
        [ScreenId.ForgotPassword] =
            navigator => new GuiForgotPassword(navigator),
        [ScreenId.AccountSettings] = navigator => new GuiSettings(navigator, false),
        [ScreenId.SettingsLanguage] = navigator => new GuiSettings(navigator, true),
        [ScreenId.Menu] = navigator => new GuiMenu(navigator),
        [ScreenId.ChangePassword] =
            navigator => new GuiChangePassword(navigator),
        [ScreenId.ChangeEmail] =
            navigator => new GuiChangeEmail(navigator),
        [ScreenId.ChangeNickname] =
            navigator => new GuiChangeNickname(navigator),
        [ScreenId.DeleteAccount] =
            navigator => new GuiDeleteAccount(navigator),
        [ScreenId.ActiveSessions] =
            navigator => new GuiActiveSessions(navigator),
        [ScreenId.Profile] =
            navigator => new GuiProfile(navigator),
        [ScreenId.EditProfile] =
            navigator => new GuiEditProfile(navigator),
        [ScreenId.PlayerCard] =
            navigator => new GuiPlayerCard(navigator),
        [ScreenId.Friends] =
            navigator => new GuiFriends(navigator),
        [ScreenId.AddFriend] =
            navigator => new GuiAddFriend(navigator),
        [ScreenId.MatchHistory] =
            navigator => new GuiMatchHistory(navigator),
        [ScreenId.CoinHistory] =
            navigator => new GuiCoinHistory(navigator),
        [ScreenId.Ranking] =
            navigator => new GuiRanking(navigator),
        [ScreenId.Report] =
            navigator => new GuiReport(navigator),
        [ScreenId.ModerationQueue] =
            navigator => new GuiModerationQueue(navigator),
        [ScreenId.ReportReview] =
            navigator => new GuiReportReview(navigator),
        [ScreenId.ApplySanction] =
            navigator => new GuiApplySanction(navigator),
        [ScreenId.Appeal] =
            navigator => new GuiAppeal(navigator),
        [ScreenId.AdminPanel] =
            navigator => new GuiAdminPanel(navigator),
        [ScreenId.Logs] =
            navigator => new GuiLogs(navigator),
        [ScreenId.Shop] =
            navigator => new GuiShop(navigator),
        [ScreenId.PurchaseConfirm] =
            navigator => new GuiPurchaseConfirm(navigator),
        [ScreenId.BoxPurchaseConfirm] =
            navigator => new GuiBoxPurchaseConfirm(navigator),
        [ScreenId.Customize] =
            navigator => new GuiCustomize(navigator),
    };

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

        if (request.Title.Length > 0)
        {
            confirm.Dialog.Title = request.Title;
        }

        confirm.Dialog.Detail = request.Detail;
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
        // The only screen that needs what the previous one produced.
        if (screen == ScreenId.RegistrationSuccess)
        {
            return new GuiRegistrationSuccess(this, argument ?? string.Empty);
        }

        if (screen == ScreenId.ResetPassword)
        {
            return new GuiResetPassword(this, argument ?? string.Empty);
        }

        return _factories.TryGetValue(screen, out Func<INavigator, IScreen>? factory)
            ? factory(this)
            : new GuiLogin(this);
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
