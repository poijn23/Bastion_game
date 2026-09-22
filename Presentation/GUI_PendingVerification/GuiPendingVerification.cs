using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_PendingVerification;

// CU-01 FA, for a Usuario still in PENDIENTE. Reached today only from
// GUI_Menu: GUI_Login does not yet branch on estado_cuenta.
public sealed class GuiPendingVerification : FormScreen
{
    private const int NoticeHeight = 48;
    private const int CardHeight = Theme.CardPadding + NoticeHeight + Theme.CardPadding;

    private readonly TextBlock _notice;
    private readonly Button _acceptButton;
    private readonly Button _resendButton;

    public GuiPendingVerification(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _notice = new TextBlock
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, NoticeHeight)
        };

        _acceptButton = CreatePrimaryButton(false);
        _resendButton = CreateSecondaryButton();
        _acceptButton.Clicked += OnAcceptClicked;
        _resendButton.Clicked += OnResendClicked;

        Register(_notice);
        Register(_acceptButton);
        Register(_resendButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.PendingVerificationSubtitle;
    }

    private void OnAcceptClicked(object? sender, EventArgs e)
    {
        Navigator.Restart(ScreenId.Login);
    }

    private void OnResendClicked(object? sender, EventArgs e)
    {
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.PendingVerificationResentBody);
    }

    protected override void ApplyTexts()
    {
        _notice.Text = string.Format(TextCatalog.PendingVerificationNoticeFormat, EmailMask.Apply(TestAccount.Email));
        _acceptButton.Title = TextCatalog.PendingVerificationAcceptButton;
        _resendButton.Title = TextCatalog.PendingVerificationResendButton;
    }
}
