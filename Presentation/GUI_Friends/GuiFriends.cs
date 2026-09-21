using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Friends;

// The friend list the social use cases return to. Removing a friend is CU-49
// and asks for confirmation, because the friendship row disappears for good.
public sealed class GuiFriends : FormScreen
{
    private const int CardHeight = 380;
    private const int RowHeight = 62;
    private const int RowGap = 10;
    private const int VisibleRows = 5;

    private readonly List<DataRow> _rows = [];
    private readonly Button _addButton;
    private readonly Button _backButton;

    public GuiFriends(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        for (int i = 0; i < VisibleRows; i++)
        {
            var row = new DataRow { Bounds = GetRowBounds(i) };
            _rows.Add(row);
            Register(row);
        }

        _addButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        _addButton.Clicked += OnAddClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_addButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.FriendsSubtitle;
    }

    private void OnAddClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AddFriend);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder rows: the friend list comes from the server.
    protected override void ApplyTexts()
    {
        for (int i = 0; i < _rows.Count; i++)
        {
            _rows[i].Title = TextCatalog.FriendsNameSample;
            _rows[i].Subtitle = IsEvenRow(i)
                ? TextCatalog.FriendsOnlineLabel
                : TextCatalog.FriendsOfflineLabel;
        }

        _addButton.Title = TextCatalog.FriendsAddButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private Rectangle GetRowBounds(int index)
    {
        int top = Card.Y + Theme.CardPadding + (index * (RowHeight + RowGap));

        return new Rectangle(ContentX, top, ContentWidth, RowHeight);
    }
}
