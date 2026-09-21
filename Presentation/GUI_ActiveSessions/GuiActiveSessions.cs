using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_ActiveSessions;

public sealed class GuiActiveSessions : FormScreen
{
    private const int RowHeight = 64;
    private const int RowGap = 12;
    private const int VisibleRows = 3;
    private const int ButtonsGap = 24;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + Theme.PanelTitleHeight + Theme.PanelGap
        + (VisibleRows * RowHeight) + ((VisibleRows - 1) * RowGap)
        + ButtonsGap + Theme.PanelButtonHeight + Theme.CardPadding;

    private readonly List<SessionRow> _rows = [];
    private readonly Button _closeAllButton;

    public GuiActiveSessions(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight, ScreenLayout.Panel)
    {
        for (int i = 0; i < VisibleRows; i++)
        {
            var row = new SessionRow
            {
                IsCurrent = i == 0,
                Bounds = new Rectangle(ContentX, PanelContentTop + (i * (RowHeight + RowGap)), ContentWidth, RowHeight)
            };
            row.CloseRequested += OnCloseRequested;
            _rows.Add(row);
            Register(row);
        }

        _closeAllButton = CreatePrimaryButton(false);
        _closeAllButton.Clicked += OnCloseAllClicked;
        Register(_closeAllButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ActiveSessionsSubtitle;
    }

    protected override void ApplyTexts()
    {
        foreach (SessionRow row in _rows)
        {
            row.Device = TextCatalog.ActiveSessionsDeviceSample;
            row.LastUse = TextCatalog.ActiveSessionsLastUseSample;
            row.CloseLabel = TextCatalog.ActiveSessionsCloseButton;
            row.CurrentLabel = TextCatalog.ActiveSessionsCurrentBadge;
        }

        _closeAllButton.Title = TextCatalog.ActiveSessionsCloseAllButton;
    }

    private void OnCloseRequested(object? sender, EventArgs e)
    {
        if (sender is SessionRow row)
        {
            Navigator.ShowConfirm(new ConfirmRequest
            {
                Body = string.Format(TextCatalog.ActiveSessionsCloseBody, row.Device, row.LastUse),
                PrimaryLabel = TextCatalog.SettingsSignOutButton,
                SecondaryLabel = TextCatalog.CommonCancelButton,
                OnConfirm = row.Hide
            });
        }
    }

    private void OnCloseAllClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.ActiveSessionsCloseAllBody,
            PrimaryLabel = TextCatalog.ActiveSessionsCloseAllButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = CloseOthers
        });
    }

    private void CloseOthers()
    {
        foreach (SessionRow row in _rows)
        {
            if (!row.IsCurrent)
            {
                row.Hide();
            }
        }

        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.ActiveSessionsChangePasswordBody,
            PrimaryLabel = TextCatalog.CommonChangeButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = () => Navigator.GoTo(ScreenId.ChangePassword)
        });
    }
}
