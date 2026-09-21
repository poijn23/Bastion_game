using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_RegistrationSuccess;

public sealed class GuiRegistrationSuccess : FormScreen
{
    private const int NoticeHeight = 96;
    private const int NoticeGap = 26;
    private const int CardHeight =
        Theme.CardPadding + NoticeHeight + NoticeGap + LabelSpace + Theme.FieldHeight + Theme.CardPadding;
    private const int MaxMaskLength = 6;

    private readonly NoticeBox _notice;
    private readonly ValueBox _emailBox;
    private readonly Button _signInButton;
    private readonly string _email;

    public GuiRegistrationSuccess(INavigator navigator, string email)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        ArgumentNullException.ThrowIfNull(email);

        _email = email;

        _notice = new NoticeBox
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, NoticeHeight)
        };

        int emailTop = _notice.Bounds.Bottom + NoticeGap + LabelSpace;
        _emailBox = new ValueBox
        {
            Bounds = new Rectangle(ContentX, emailTop, ContentWidth, Theme.FieldHeight)
        };

        _signInButton = CreatePrimaryButton(true);
        _signInButton.Clicked += OnSignInClicked;

        Register(_notice);
        Register(_emailBox);
        Register(_signInButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.RegistrationSuccessSubtitle;
    }

    protected override void ApplyTexts()
    {
        _notice.Text = TextCatalog.RegistrationSuccessNotice;
        _emailBox.Label = TextCatalog.RegistrationSuccessEmailLabel;
        _emailBox.Value = Mask(_email);
        _signInButton.Title = TextCatalog.RegistrationSuccessSignInButton;
        _signInButton.Subtitle = TextCatalog.RegistrationSuccessDetail;
    }

    private void OnSignInClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Login);
    }

    private static string Mask(string email)
    {
        int at = email.IndexOf('@');

        if (at < 2)
        {
            return email;
        }

        string local = email[..at];
        int hidden = Math.Min(local.Length - 2, MaxMaskLength);

        return local[0] + new string('*', hidden) + local[^1] + email[at..];
    }
}
