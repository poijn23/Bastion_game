using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_AddFriend;

// CU-30 main flow step 1. One field that accepts the nickname or the friend
// code, plus the people the player has met recently (CU-30 RN-07).
public sealed class GuiAddFriend : FormScreen
{
    private const int CardHeight = 380;
    private const int MaxSearchLength = 30;
    private const int RowHeight = 58;
    private const int RowGap = 10;
    private const int VisibleRows = 3;
    private const int ListGap = 26;

    private readonly TextField _searchField;
    private readonly List<DataRow> _rows = [];
    private readonly Button _searchButton;
    private readonly Button _backButton;

    public GuiAddFriend(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _searchField = new TextField { MaxLength = MaxSearchLength, Bounds = GetRow(0) };

        for (int i = 0; i < VisibleRows; i++)
        {
            var row = new DataRow { Bounds = GetSuggestionBounds(i) };
            _rows.Add(row);
            Register(row);
        }

        _searchButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        _searchButton.Clicked += OnSearchClicked;
        _backButton.Clicked += OnBackClicked;

        RegisterField(_searchField);
        Register(_searchButton);
        Register(_backButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.AddFriendSubtitle;
    }

    private void OnSearchClicked(object? sender, EventArgs e)
    {
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder suggestions: they come from the server.
    private void ApplyTexts()
    {
        _searchField.Label = TextCatalog.AddFriendSearchLabel;
        _searchField.Placeholder = TextCatalog.AddFriendSearchPlaceholder;

        foreach (DataRow row in _rows)
        {
            row.Title = TextCatalog.AddFriendSuggestionSample;
            row.Subtitle = TextCatalog.AddFriendSuggestionReason;
        }

        _searchButton.Title = TextCatalog.AddFriendSearchButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private Rectangle GetSuggestionBounds(int index)
    {
        int top = GetRow(0).Bottom + ListGap + (index * (RowHeight + RowGap));

        return new Rectangle(ContentX, top, ContentWidth, RowHeight);
    }
}
