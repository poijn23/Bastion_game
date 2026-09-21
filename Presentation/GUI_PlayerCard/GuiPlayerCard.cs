using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_PlayerCard;

// CU-16 main flow step 4. Another player seen from outside: the head to head
// score, the last matches together and what can be done about them.
public sealed class GuiPlayerCard : FormScreen
{
    private const int CardHeight = 392;
    private const int AvatarSize = 88;
    private const int SectionGap = 20;
    private const int RowHeight = 58;
    private const int RowGap = 10;
    private const int VisibleRows = 3;
    private const int ScoreWidth = 200;
    private const int ScoreHeight = 88;

    private readonly AvatarBox _avatar;
    private readonly StatTile _headToHead;
    private readonly List<DataRow> _rows = [];
    private readonly Button _addFriendButton;
    private readonly Button _backButton;

    public GuiPlayerCard(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _avatar = new AvatarBox { Bounds = new Rectangle(ContentX, top, AvatarSize, AvatarSize) };
        _headToHead = new StatTile
        {
            Bounds = new Rectangle(Card.Right - Theme.CardPadding - ScoreWidth, top, ScoreWidth, ScoreHeight)
        };

        for (int i = 0; i < VisibleRows; i++)
        {
            var row = new DataRow { Bounds = GetRowBounds(i, top + AvatarSize + SectionGap) };
            _rows.Add(row);
            Register(row);
        }

        _addFriendButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        LayOutActionsInRow([_addFriendButton, _backButton]);
        _addFriendButton.Clicked += OnAddFriendClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_avatar);
        Register(_headToHead);
        Register(_addFriendButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.PlayerCardSubtitle;
    }

    private void OnAddFriendClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AddFriend);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder rows: the shared matches come from the server.
    protected override void ApplyTexts()
    {
        _avatar.IconLabel = TextCatalog.ProfileAvatarPlaceholder;
        _headToHead.Caption = TextCatalog.PlayerCardHeadToHeadCaption;
        _headToHead.Value = TextCatalog.PlayerCardHeadToHeadSample;

        foreach (DataRow row in _rows)
        {
            row.Title = TextCatalog.PlayerCardMatchSample;
            row.Subtitle = TextCatalog.MatchHistoryRowSample;
        }

        _addFriendButton.Title = TextCatalog.PlayerCardAddFriendButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private Rectangle GetRowBounds(int index, int top)
    {
        return new Rectangle(ContentX, top + (index * (RowHeight + RowGap)), ContentWidth, RowHeight);
    }
}
