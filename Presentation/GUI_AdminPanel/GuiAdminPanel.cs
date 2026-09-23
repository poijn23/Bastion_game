using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_AdminPanel;

// CU-46 main flow step 3. The candidate file an administrator reads before
// granting the role: how long the account has existed and what it carries.
public sealed class GuiAdminPanel : FormScreen
{
    private const int CardHeight = 392;
    private const int SectionGap = 20;
    private const int RowHeight = 54;
    private const int RowGap = 8;
    private const int VisibleRows = 2;

    private readonly ValueBox _nicknameBox;
    private readonly ValueBox _registeredBox;
    private readonly ValueBox _stateBox;
    private readonly ValueBox _typeBox;
    private readonly List<DataRow> _history = [];
    private readonly Button _grantButton;
    private readonly Button _queueButton;
    private readonly Button _logsButton;
    private readonly Button _backButton;

    public GuiAdminPanel(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int column = (ContentWidth - SectionGap) / 2;

        _nicknameBox = CreateValueBox(new Rectangle(ContentX, FirstRowTop, column, Theme.FieldHeight));
        _registeredBox = CreateValueBox(
            new Rectangle(ContentX + column + SectionGap, FirstRowTop, column, Theme.FieldHeight));
        _stateBox = CreateValueBox(new Rectangle(ContentX, FirstRowTop + RowSpacing, column, Theme.FieldHeight));
        _typeBox = CreateValueBox(
            new Rectangle(ContentX + column + SectionGap, FirstRowTop + RowSpacing, column, Theme.FieldHeight));

        int historyTop = FirstRowTop + (RowSpacing * 2);

        for (int i = 0; i < VisibleRows; i++)
        {
            var row = new DataRow
            {
                Bounds = new Rectangle(ContentX, historyTop + (i * (RowHeight + RowGap)), ContentWidth, RowHeight)
            };

            _history.Add(row);
            Register(row);
        }

        _grantButton = CreatePrimaryButton(false);
        _queueButton = CreateSecondaryButton();
        _logsButton = CreateSecondaryButton();
        _backButton = CreateSecondaryButton();
        LayOutActionsInRow([_grantButton, _queueButton, _logsButton, _backButton]);
        _grantButton.Clicked += OnGrantClicked;
        _queueButton.Clicked += OnQueueClicked;
        _logsButton.Clicked += OnLogsClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_nicknameBox);
        Register(_registeredBox);
        Register(_stateBox);
        Register(_typeBox);
        Register(_grantButton);
        Register(_queueButton);
        Register(_logsButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.AdminPanelSubtitle;
    }

    private void OnGrantClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.AdminPanelGrantConfirmBody,
            PrimaryLabel = TextCatalog.AdminPanelGrantButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = GoBack
        });
    }

    private void OnQueueClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ModerationQueue);
    }

    private void OnLogsClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Logs);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void GoBack()
    {
        Navigator.GoBack();
    }

    // Placeholder file: the candidate comes from the server.
    protected override void ApplyTexts()
    {
        _nicknameBox.Label = TextCatalog.AdminPanelNicknameLabel;
        _nicknameBox.Value = TextCatalog.RankingPlayerSample;
        _registeredBox.Label = TextCatalog.AdminPanelRegisteredLabel;
        _registeredBox.Value = TextCatalog.AdminPanelRegisteredSample;
        _stateBox.Label = TextCatalog.AdminPanelStateLabel;
        _stateBox.Value = TextCatalog.AdminPanelStateSample;
        _typeBox.Label = TextCatalog.AdminPanelTypeLabel;
        _typeBox.Value = TextCatalog.AdminPanelTypeSample;

        foreach (DataRow row in _history)
        {
            row.Title = TextCatalog.AdminPanelHistorySample;
            row.Subtitle = TextCatalog.ApplySanctionHistoryDateSample;
        }

        _grantButton.Title = TextCatalog.AdminPanelGrantButton;
        _queueButton.Title = TextCatalog.AdminPanelQueueButton;
        _logsButton.Title = TextCatalog.AdminPanelLogsButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }
}
