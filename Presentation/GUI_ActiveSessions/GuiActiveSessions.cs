using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_ActiveSessions;

// CU-10 main flow step 4. The current session is marked and has no close
// button: it cannot be closed from here.
public sealed class GuiActiveSessions : FormScreen
{
    private const int CardHeight = 356;
    private const int RowHeight = 64;
    private const int RowGap = 12;
    private const int VisibleRows = 4;

    private readonly List<SessionRow> _rows = [];
    private readonly Button _closeAllButton;
    private readonly Button _backButton;

    public GuiActiveSessions(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        for (int i = 0; i < VisibleRows; i++)
        {
            var row = new SessionRow
            {
                IsCurrent = i == 0,
                Bounds = GetRowBounds(i)
            };

            row.CloseRequested += OnCloseRequested;
            _rows.Add(row);
            Register(row);
        }

        _closeAllButton = CreatePrimaryButton(false);
        _backButton = CreateSecondaryButton();
        _closeAllButton.Clicked += OnCloseAllClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_closeAllButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ActiveSessionsSubtitle;
    }

    private void OnCloseRequested(object? sender, EventArgs e)
    {
    }

    private void OnCloseAllClicked(object? sender, EventArgs e)
    {
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // The rows are placeholders until the session list arrives from the server.
    private void ApplyTexts()
    {
        foreach (SessionRow row in _rows)
        {
            row.Device = TextCatalog.ActiveSessionsDeviceSample;
            row.LastUse = TextCatalog.ActiveSessionsLastUseSample;
            row.CloseLabel = TextCatalog.ActiveSessionsCloseButton;
            row.CurrentLabel = TextCatalog.ActiveSessionsCurrentBadge;
        }

        _closeAllButton.Title = TextCatalog.ActiveSessionsCloseAllButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private Rectangle GetRowBounds(int index)
    {
        int top = Card.Y + Theme.CardPadding + (index * (RowHeight + RowGap));

        return new Rectangle(ContentX, top, ContentWidth, RowHeight);
    }
}
