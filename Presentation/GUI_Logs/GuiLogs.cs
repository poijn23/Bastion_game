using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Logs;

// CU-48 main flow steps 1 and 4. Which log, the date range and the filters on
// top; the records and their counts below.
public sealed class GuiLogs : FormScreen
{
    private const int SectionGap = 20;
    private const int RowHeight = 50;
    private const int RowGap = 8;
    private const int VisibleRows = 3;

    // Derived from the filter row plus the result rows, so the card ends where
    // the list ends instead of leaving the bottom half empty.
    private const int CardHeight = (Theme.CardPadding * 2) + (LabelSpace * 2) + Theme.FieldHeight
        + SectionGap + (VisibleRows * RowHeight) + ((VisibleRows - 1) * RowGap);

    private readonly Selector _logSelector;
    private readonly TextField _fromField;
    private readonly TextField _toField;
    private readonly List<DataRow> _rows = [];
    private readonly Button _searchButton;
    private readonly Button _backButton;

    public GuiLogs(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int column = (ContentWidth - (SectionGap * 2)) / 3;

        _logSelector = new Selector
        {
            Options = BuildLogs(),
            Bounds = new Rectangle(ContentX, FirstRowTop, column, Theme.FieldHeight)
        };

        _fromField = CreateDateField(ContentX + column + SectionGap, column);
        _toField = CreateDateField(ContentX + ((column + SectionGap) * 2), column);

        int rowsTop = FirstRowTop + Theme.FieldHeight + SectionGap + LabelSpace;

        for (int i = 0; i < VisibleRows; i++)
        {
            var row = new DataRow
            {
                Bounds = new Rectangle(ContentX, rowsTop + (i * (RowHeight + RowGap)), ContentWidth, RowHeight)
            };

            _rows.Add(row);
            Register(row);
        }

        _searchButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        LayOutActionsInRow([_searchButton, _backButton]);
        _searchButton.Clicked += OnSearchClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_logSelector);
        RegisterField(_fromField);
        RegisterField(_toField);
        Register(_searchButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.LogsSubtitle;
    }

    private void OnSearchClicked(object? sender, EventArgs e)
    {
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder records: the log comes from the server.
    protected override void ApplyTexts()
    {
        _logSelector.Label = TextCatalog.LogsWhichLabel;
        _logSelector.Options = BuildLogs();
        _fromField.Label = TextCatalog.LogsFromLabel;
        _fromField.Placeholder = TextCatalog.LogsDatePlaceholder;
        _toField.Label = TextCatalog.LogsToLabel;
        _toField.Placeholder = TextCatalog.LogsDatePlaceholder;

        foreach (DataRow row in _rows)
        {
            row.Title = TextCatalog.LogsRecordSample;
            row.Subtitle = TextCatalog.LogsRecordDetailSample;
            row.Value = TextCatalog.LogsResultSample;
        }

        _searchButton.Title = TextCatalog.LogsSearchButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private TextField CreateDateField(int x, int width)
    {
        return new TextField
        {
            IsCentered = true,
            Bounds = new Rectangle(x, FirstRowTop, width, Theme.FieldHeight)
        };
    }

    private static IReadOnlyList<string> BuildLogs()
    {
        return [TextCatalog.LogsAccessOption, TextCatalog.LogsModerationOption];
    }
}
